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
            WaitAndRetryBackoffType.Constant => Backoff.ConstantBackoff(configuration.BackOffDelay, int.MaxValue),
            WaitAndRetryBackoffType.Linear => Backoff.LinearBackoff(configuration.BackOffDelay, int.MaxValue, configuration.Factor),
            WaitAndRetryBackoffType.Exponential => Backoff.ExponentialBackoff(configuration.BackOffDelay, int.MaxValue,
                configuration.Factor),
            WaitAndRetryBackoffType.AwsDecorrelatedJitterBackoff => Backoff.DecorrelatedJitterBackoffV2(configuration.BackOffDelay,
                int.MaxValue),
            WaitAndRetryBackoffType.DecorrelatedJitterBackoffV2 => Backoff.DecorrelatedJitterBackoffV2(configuration.BackOffDelay,
                int.MaxValue),
            _ => throw new ArgumentOutOfRangeException(nameof(configuration.BackoffType),
                $"{nameof(configuration.BackoffType)} is unsupported")
        };
    }
}
