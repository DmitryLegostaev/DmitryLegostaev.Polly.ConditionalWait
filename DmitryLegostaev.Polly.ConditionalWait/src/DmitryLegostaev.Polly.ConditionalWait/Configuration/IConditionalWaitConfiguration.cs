using Polly.Retry;

namespace DmitryLegostaev.Polly.ConditionalWait.Configuration;

public interface IConditionalWaitConfiguration
{
    public TimeSpan Timeout { get; }
    public TimeSpan BackOffDelay { get; }
    public double Factor { get; }
    public RetryBackoffType BackoffType { get; }
    // public int RetryCount { get; }
}
