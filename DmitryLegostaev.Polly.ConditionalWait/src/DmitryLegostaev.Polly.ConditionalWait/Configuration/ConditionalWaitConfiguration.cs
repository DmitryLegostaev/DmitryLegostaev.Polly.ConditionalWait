using DmitryLegostaev.Polly.ConditionalWait.Enums;

namespace DmitryLegostaev.Polly.ConditionalWait.Configuration;

public class ConditionalWaitConfiguration : IConditionalWaitConfiguration
{
    public ConditionalWaitConfiguration(TimeSpan Timeout, TimeSpan BackOffDelay, double? Factor = 1.0, WaitAndRetryBackoffType? BackoffType = WaitAndRetryBackoffType.Constant)
    {
        this.Timeout = Timeout;
        this.BackOffDelay = BackOffDelay;
        if (Factor is not null) this.Factor = (double) Factor;
        if (BackoffType is not null) this.BackoffType = (WaitAndRetryBackoffType) BackoffType;
    }
    
    public TimeSpan Timeout { get; set; }
    public TimeSpan BackOffDelay { get; set; }
    public double Factor { get; set; }
    public WaitAndRetryBackoffType BackoffType { get; set; }
}
