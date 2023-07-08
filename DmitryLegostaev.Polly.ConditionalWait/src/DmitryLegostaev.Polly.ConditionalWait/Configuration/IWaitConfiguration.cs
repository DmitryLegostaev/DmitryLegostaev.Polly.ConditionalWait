using DmitryLegostaev.Polly.ConditionalWait.Enums;

namespace DmitryLegostaev.Polly.ConditionalWait.Configuration;

public interface IWaitConfiguration
{
    public int RetryCount { get; }
    public TimeSpan Timeout { get; }
    public TimeSpan BackOffDelay { get; }
    public double Factor { get; }
    public WaitAndRetryBackoffType BackoffType { get; }
}
