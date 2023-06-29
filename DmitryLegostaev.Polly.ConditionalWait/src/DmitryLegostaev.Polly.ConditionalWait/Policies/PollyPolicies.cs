using DmitryLegostaev.Polly.ConditionalWait.Configuration;
using DmitryLegostaev.Polly.HandleFromList.Extensions;
using Humanizer;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;
using Polly.Timeout;

namespace DmitryLegostaev.Polly.ConditionalWait.Policies;

public static class PollyPolicies
{
    public static Policy<T> ConditionalWaitPolicy<T>(Func<T, bool> handleResultDelegate, Func<T> codeToExecute,
        IConditionalWaitConfiguration waitConfiguration, IList<Type>? exceptionsToIgnore = null, string? failReason = null,
        string? codePurpose = null)
    {
        var negatedHandleResultDelegateForPolly = NegateFuncTBoolResult(handleResultDelegate);

        var handleResultPolicyBuilder = Policy<T>
            .HandleResult(negatedHandleResultDelegateForPolly)
            .OrFromList(exceptionsToIgnore);

        var waitAndRetryPolicy = handleResultPolicyBuilder
            .WaitAndRetry(
                CalculateBackoff(waitConfiguration),
                (_, _, arg3, _) =>
                {
                    LogManager.GetCurrentClassLogger()
                        .Debug($"Unexpected code execution result. Retry #{arg3} (Execution #{arg3 + 1}):");
                });

        var timeoutPolicy = Policy
            .Timeout(waitConfiguration.Timeout, TimeoutStrategy.Optimistic);

        var timeoutRejectedFallbackPolicy = Policy<T>
            .Handle<TimeoutRejectedException>()
            .Fallback(codeToExecute.Invoke);

        var timeoutExceptionMessage = BuildExceptionMessage(waitConfiguration.Timeout, codePurpose, failReason);
        var timeoutExceededAndUnexpectedResultException = new TimeoutException(timeoutExceptionMessage);
        var unexpectedResultFallbackPolicy = handleResultPolicyBuilder
            .Fallback(() => throw timeoutExceededAndUnexpectedResultException);

        return unexpectedResultFallbackPolicy.Wrap(timeoutRejectedFallbackPolicy.Wrap(timeoutPolicy.Wrap(waitAndRetryPolicy)));
    }

    private static IEnumerable<TimeSpan>? CalculateBackoff(IConditionalWaitConfiguration configuration)
    {
        // LogManager.GetCurrentClassLogger().Debug($"Executing {nameof(CalculateBackoff)} method. {configuration.Dump()}");
        switch (configuration.BackoffType)
        {
            case RetryBackoffType.Constant:
                return Backoff.ConstantBackoff(configuration.BackOffDelay, int.MaxValue);
            case RetryBackoffType.Linear:
                return Backoff.LinearBackoff(configuration.BackOffDelay, int.MaxValue, configuration.Factor);
            case RetryBackoffType.Exponential:
                return Backoff.ExponentialBackoff(configuration.BackOffDelay, int.MaxValue, configuration.Factor);
            case RetryBackoffType.ExponentialWithJitter:
                return Backoff.(configuration.BackOffDelay, int.MaxValue, configuration.Factor);
            default:
                throw new ArgumentOutOfRangeException(nameof(configuration.BackoffType), $"{nameof(configuration.BackoffType)} is unsupported");
        }
    }

    private static Func<T, bool> NegateFuncTBoolResult<T>(Func<T, bool> conditionPredicate)
    {
        return t => !conditionPredicate(t);
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
