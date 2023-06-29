using Polly.Retry;

namespace DmitryLegostaev.Polly.ConditionalWait.Configuration;

public class ConditionalWaitConfiguration : IConditionalWaitConfiguration
{
    public ConditionalWaitConfiguration(TimeSpan Timeout, TimeSpan BackOffDelay, double? Factor = 1.0, RetryBackoffType? BackoffType = RetryBackoffType.Constant)
    {
        this.Timeout = Timeout;
        this.BackOffDelay = BackOffDelay;
        if (Factor is not null) this.Factor = (double) Factor;
        if (BackoffType is not null) this.BackoffType = (RetryBackoffType) BackoffType;
    }
    
    public TimeSpan Timeout { get; set; }
    public TimeSpan BackOffDelay { get; set; }
    public double Factor { get; set; }
    public RetryBackoffType BackoffType { get; set; }
    //
    // // Adding +1 to be sure that overall retry time will exceed timeout
    // // Works for constant, linear and exponential timeouts with positive factors (> 1.0)
    // // I find this approach is most suitable, flexible and resource optimized for ConditionalWait implementation
    // // We can add some code to handle negative factors (< 1.0), but it's not necessary
    // public int RetryCount => (int) Timeout.Divide(BackOffDelay) + 1;
}
