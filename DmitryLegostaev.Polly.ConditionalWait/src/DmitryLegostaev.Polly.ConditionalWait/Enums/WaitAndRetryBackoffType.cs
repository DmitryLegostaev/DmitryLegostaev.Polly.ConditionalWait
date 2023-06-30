namespace DmitryLegostaev.Polly.ConditionalWait.Enums;

public enum WaitAndRetryBackoffType
{
    Constant,
    Linear,
    Exponential,
    AwsDecorrelatedJitterBackoff,
    DecorrelatedJitterBackoffV2
}
