using DmitryLegostaev.Polly.ConditionalWait.Configuration;
using DmitryLegostaev.Polly.ConditionalWait.Enums;
using Polly.Contrib.WaitAndRetry;

namespace DmitryLegostaev.Polly.ConditionalWait.Utilities;

public static class BackoffUtilities
{
    public static IEnumerable<TimeSpan>? CalculateBackoff(IConditionalWaitConfiguration configuration)
    {
        switch (configuration.BackoffType)
        {
            case WaitAndRetryBackoffType.Constant:
                return Backoff.ConstantBackoff(configuration.BackOffDelay, int.MaxValue);
            case WaitAndRetryBackoffType.Linear:
                return Backoff.LinearBackoff(configuration.BackOffDelay, int.MaxValue, configuration.Factor);
            case WaitAndRetryBackoffType.Exponential:
                return Backoff.ExponentialBackoff(configuration.BackOffDelay, int.MaxValue, configuration.Factor);
            case WaitAndRetryBackoffType.AwsDecorrelatedJitterBackoff:
                return Backoff.DecorrelatedJitterBackoffV2(configuration.BackOffDelay, int.MaxValue);
            case WaitAndRetryBackoffType.DecorrelatedJitterBackoffV2:
                return Backoff.DecorrelatedJitterBackoffV2(configuration.BackOffDelay, int.MaxValue);
            default:
                throw new ArgumentOutOfRangeException(nameof(configuration.BackoffType), $"{nameof(configuration.BackoffType)} is unsupported");
        }
    }
}
