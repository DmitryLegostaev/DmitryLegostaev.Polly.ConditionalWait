using DmitryLegostaev.Polly.ConditionalWait.Configuration;
using DmitryLegostaev.Polly.ConditionalWait.Enums;
using Polly.Contrib.WaitAndRetry;

namespace DmitryLegostaev.Polly.ConditionalWait.Utilities;

public static class BackoffUtilities
{
    public static IEnumerable<TimeSpan>? CalculateBackoff(IWaitConfiguration configuration)
    {
        return configuration.BackoffType switch
        {
            WaitAndRetryBackoffType.Constant => Backoff.ConstantBackoff(configuration.BackOffDelay, configuration.RetryCount),
            WaitAndRetryBackoffType.Linear => Backoff.LinearBackoff(configuration.BackOffDelay, configuration.RetryCount, configuration.Factor),
            WaitAndRetryBackoffType.Exponential => Backoff.ExponentialBackoff(configuration.BackOffDelay, configuration.RetryCount,
                configuration.Factor),
            WaitAndRetryBackoffType.AwsDecorrelatedJitterBackoff => Backoff.DecorrelatedJitterBackoffV2(configuration.BackOffDelay,
                configuration.RetryCount),
            WaitAndRetryBackoffType.DecorrelatedJitterBackoffV2 => Backoff.DecorrelatedJitterBackoffV2(configuration.BackOffDelay,
                configuration.RetryCount),
            _ => throw new ArgumentOutOfRangeException(nameof(configuration.BackoffType),
                $"{nameof(configuration.BackoffType)} is unsupported")
        };
    }
}
