using DmitryLegostaev.Polly.ConditionalWait.Configuration;
using DmitryLegostaev.Polly.ConditionalWait.Utilities;
using DmitryLegostaev.Polly.HandleFromList.Extensions;
using Humanizer;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;

namespace DmitryLegostaev.Polly.ConditionalWait.Policies;

public static class PollyPolicies
{
    public static Policy<T> ConditionalWaitPolicy<T>(Func<T, bool> handleResultDelegate, Func<T> codeToExecute,
        IWaitConfiguration waitConfiguration,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null)
    {
        var negatedHandleResultDelegateForPolly = PredicatesUtilities.NegateFuncTBoolResult(handleResultDelegate);

        var handleResultPolicyBuilder = Policy<T>
            .HandleResult(negatedHandleResultDelegateForPolly)
            .OrFromList(exceptionsToIgnore);

        var waitAndRetryPolicy = handleResultPolicyBuilder
            .WaitAndRetry(
                BackoffUtilities.CalculateBackoff(waitConfiguration),
                (result, span, i, arg4) =>
                {
                    logger?.LogDebug("An unexpected code execution result occured. Retry #{RetryAttempt} (Execution #{ExecutionAttempt}): {CodePurpose}",
                        i, i + 1, codePurpose);
                });

        var timeoutPolicy = Policy
            .Timeout(waitConfiguration.Timeout, TimeoutStrategy.Optimistic);

        var timeoutRejectedFallbackPolicy = Policy<T>
            .Handle<TimeoutRejectedException>()
            .Fallback(() =>
            {
                var codeExecuteResult = codeToExecute.Invoke();
                logger?.LogDebug("The last attempt to execute the code has been completed");
                return codeExecuteResult;
            });

        var timeoutExceededAndUnexpectedResultException =
            new TimeoutException(
                $"ConditionalWait timed out after {waitConfiguration.Timeout.Humanize()}. Fail reason: {failReason ?? "Not Specified"}. Executed code purpose: {codePurpose ?? "NotSpecified"}");
        var unexpectedResultFallbackPolicy = handleResultPolicyBuilder
            .Fallback(() => throw timeoutExceededAndUnexpectedResultException);

        return unexpectedResultFallbackPolicy.Wrap(timeoutRejectedFallbackPolicy.Wrap(timeoutPolicy.Wrap(waitAndRetryPolicy)));
    }
}
