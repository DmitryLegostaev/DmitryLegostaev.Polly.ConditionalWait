using DmitryLegostaev.Polly.ConditionalWait.Configuration;
using DmitryLegostaev.Polly.ConditionalWait.Policies;
using DmitryLegostaev.Polly.ConditionalWait.Predicates;
using Microsoft.Extensions.Logging;

namespace DmitryLegostaev.Polly.ConditionalWait;

public class ConditionalWait : IConditionalWait
{
    public ConditionalWait(TimeSpan? defaultTimeout = null, TimeSpan? defaultBackOffDelay = null)
    {
        TimeSpan GetTimeSpanValue(TimeSpan? initialTimeSpanValue, string environmentVariableName, TimeSpan defaultTimeSpan) =>
            initialTimeSpanValue ??
            (TimeSpan.TryParse(Environment.GetEnvironmentVariable(environmentVariableName),
                out var parsedTimeSpanFromEnvironmentVariables)
                ? parsedTimeSpanFromEnvironmentVariables
                : defaultTimeSpan);

        this.defaultTimeout = GetTimeSpanValue(defaultTimeout, DefaultTimeoutEnvironmentVariableName, TimeSpan.FromSeconds(30));

        this.defaultBackOffDelay =
            GetTimeSpanValue(defaultBackOffDelay, DefaultBackOffDelayEnvironmentVariableName, TimeSpan.FromMilliseconds(300));
    }

    private readonly TimeSpan defaultTimeout;
    private readonly TimeSpan defaultBackOffDelay;
    private const string DefaultTimeoutEnvironmentVariableName = $"{nameof(ConditionalWait)}__{nameof(defaultTimeout)}";
    private const string DefaultBackOffDelayEnvironmentVariableName = $"{nameof(ConditionalWait)}__{nameof(defaultBackOffDelay)}";

    public T WaitForPredicateAndGetResult<T>(Func<T> codeToExecute, Func<T, bool> conditionPredicate,
        TimeSpan? timeout = null, TimeSpan? backoffDelay = null,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null)
    {
        var configuration = InitConditionalWaitConfiguration(timeout, backoffDelay);

        return WaitForPredicateAndGetResult(codeToExecute, conditionPredicate, configuration, exceptionsToIgnore, failReason, codePurpose,
            logger);
    }

    public T WaitForPredicateAndGetResult<T>(Func<T> codeToExecute, Func<T, bool> conditionPredicate,
        IConditionalWaitConfiguration waitConfiguration,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null)
    {
        return PollyPolicies
            .ConditionalWaitPolicy(conditionPredicate, codeToExecute, waitConfiguration, exceptionsToIgnore, failReason, codePurpose,
                logger)
            .Execute(codeToExecute);
    }

    public T WaitForAndGetResult<T>(Func<T> codeToExecute,
        TimeSpan? timeout = null, TimeSpan? backoffDelay = null,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null)
    {
        var configuration = InitConditionalWaitConfiguration(timeout, backoffDelay);

        return WaitForAndGetResult(codeToExecute, configuration, exceptionsToIgnore, failReason, codePurpose, logger);
    }

    public T WaitForAndGetResult<T>(Func<T> codeToExecute,
        IConditionalWaitConfiguration waitConfiguration,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null)
    {
        var conditionPredicate = PollyPredicates.IsNotNullPredicate<T>();

        return PollyPolicies
            .ConditionalWaitPolicy(conditionPredicate, codeToExecute, waitConfiguration, exceptionsToIgnore, failReason, codePurpose,
                logger)
            .Execute(codeToExecute);
    }

    public void WaitForTrue(Func<bool> codeToExecute,
        TimeSpan? timeout = null, TimeSpan? backoffDelay = null,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null)
    {
        var configuration = InitConditionalWaitConfiguration(timeout, backoffDelay);

        WaitForTrue(codeToExecute, configuration, exceptionsToIgnore, failReason, codePurpose, logger);
    }

    public void WaitForTrue(Func<bool> codeToExecute,
        IConditionalWaitConfiguration waitConfiguration,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null)
    {
        var conditionPredicate = PollyPredicates.IsTruePredicate;

        PollyPolicies
            .ConditionalWaitPolicy(conditionPredicate, codeToExecute, waitConfiguration, exceptionsToIgnore, failReason, codePurpose,
                logger)
            .Execute(codeToExecute);
    }

    private IConditionalWaitConfiguration InitConditionalWaitConfiguration(TimeSpan? timeout = null,
        TimeSpan? backoffDelay = null)
    {
        return new ConditionalWaitConfiguration(timeout ?? defaultTimeout, backoffDelay ?? defaultBackOffDelay);
    }
}
