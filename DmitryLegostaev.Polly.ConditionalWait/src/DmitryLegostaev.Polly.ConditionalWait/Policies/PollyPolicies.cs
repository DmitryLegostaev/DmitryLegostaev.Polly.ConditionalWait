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
        IConditionalWaitConfiguration waitConfiguration,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null)
    {
        var negatedHandleResultDelegateForPolly = PredicatesUtilities.NegateFuncTBoolResult(handleResultDelegate);

        var handleResultPolicyBuilder = Policy<T>
            .HandleResult(negatedHandleResultDelegateForPolly)
            .OrFromList(exceptionsToIgnore);

        var waitAndRetryPolicy = handleResultPolicyBuilder
            .WaitAndRetry(
                BackoffUtilities.CalculateBackoff(waitConfiguration),
                (_, _, arg3, _) =>
                {
                    logger?.LogDebug("An unexpected code execution result occured. Retry #{RetryAttempt} (Execution #{ExecutionAttempt}): {CodePurpose}",
                        arg3, arg3 + 1, codePurpose);
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
            new TimeoutException(BuildExceptionMessage(waitConfiguration.Timeout, codePurpose, failReason));
        var unexpectedResultFallbackPolicy = handleResultPolicyBuilder
            .Fallback(() => throw timeoutExceededAndUnexpectedResultException);

        return unexpectedResultFallbackPolicy.Wrap(timeoutRejectedFallbackPolicy.Wrap(timeoutPolicy.Wrap(waitAndRetryPolicy)));
    }

    private static string BuildExceptionMessage(TimeSpan timeout, string? codePurpose = null, string? failReason = null)
    {
        var timeoutExceptionMessage = $"ConditionalWait timed out after {timeout.Humanize()}.";

        if (failReason is not null)
        {
            timeoutExceptionMessage += $" Fail reason: {failReason}.";
        }

        if (codePurpose is not null)
        {
            timeoutExceptionMessage += $" Executed code purpose: {codePurpose}.";
        }

        return timeoutExceptionMessage;
    }
}
