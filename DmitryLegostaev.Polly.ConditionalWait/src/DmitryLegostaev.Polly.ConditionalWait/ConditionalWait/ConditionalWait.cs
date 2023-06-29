using DmitryLegostaev.Polly.ConditionalWait.Configuration;
using DmitryLegostaev.Polly.ConditionalWait.Policies;
using DmitryLegostaev.Polly.ConditionalWait.Predicates;
using Polly;

namespace DmitryLegostaev.Polly.ConditionalWait.ConditionalWait;

public class ConditionalWait : IConditionalWait
{
    public T WaitForPredicateAndGetResult<T>(Func<T> codeToExecute, Func<T, bool> conditionPredicate, TimeSpan? timeout = null, TimeSpan? backoffDelay = null,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null)
    {
        var configuration = InitConditionalWaitConfigurationModel(timeout, backoffDelay);

        return WaitForPredicateAndGetResult(codeToExecute, conditionPredicate, configuration, exceptionsToIgnore, failReason, codePurpose);
    }

    public T WaitForPredicateAndGetResult<T>(Func<T> codeToExecute, Func<T, bool> conditionPredicate, IConditionalWaitConfiguration waitConfiguration,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null)
    {
        return PollyPolicies
            .ConditionalWaitPolicy(conditionPredicate, codeToExecute, waitConfiguration, exceptionsToIgnore, failReason, codePurpose)
            .Execute(codeToExecute);
    }

    public T WaitForAndGetResult<T>(Func<T> codeToExecute, TimeSpan? timeout = null, TimeSpan? backoffDelay = null, IList<Type>? exceptionsToIgnore = null,
        string? failReason = null, string? codePurpose = null)
    {
        var configuration = InitConditionalWaitConfigurationModel(timeout, backoffDelay);

        return WaitForAndGetResult(codeToExecute, configuration, exceptionsToIgnore, failReason, codePurpose);
    }

    public T WaitForAndGetResult<T>(Func<T> codeToExecute, IConditionalWaitConfiguration waitConfiguration, IList<Type>? exceptionsToIgnore = null,
        string? failReason = null, string? codePurpose = null)
    {
        var conditionPredicate = PollyPredicates.IsNotNullPredicate<T>();

        return PollyPolicies
            .ConditionalWaitPolicy(conditionPredicate, codeToExecute, waitConfiguration, exceptionsToIgnore, failReason, codePurpose)
            .Execute(codeToExecute);
    }

    public void WaitForTrue(Func<bool> codeToExecute, TimeSpan? timeout = null, TimeSpan? backoffDelay = null, IList<Type>? exceptionsToIgnore = null,
        string? failReason = null, string? codePurpose = null)
    {
        var configuration = InitConditionalWaitConfigurationModel(timeout, backoffDelay);

        WaitForTrue(codeToExecute, configuration, exceptionsToIgnore, failReason, codePurpose);
    }

    public void WaitForTrue(Func<bool> codeToExecute, IConditionalWaitConfiguration waitConfiguration, IList<Type>? exceptionsToIgnore = null,
        string? failReason = null, string? codePurpose = null)
    {
        var conditionPredicate = PollyPredicates.IsTruePredicate;

        PollyPolicies
            .ConditionalWaitPolicy(conditionPredicate, codeToExecute, waitConfiguration, exceptionsToIgnore, failReason, codePurpose)
            .Execute(codeToExecute);
    }
    
    private static IConditionalWaitConfiguration InitConditionalWaitConfigurationModel(TimeSpan? timeout = null,
        TimeSpan? backoffDelay = null)
    {
        if (timeout is not null)
        {
            configuration.Timeout = (TimeSpan) timeout;
        }

        if (backoffDelay is not null)
        {
            configuration.BackOffDelay = (TimeSpan) backoffDelay;
        }

        return new ConditionalWaitConfiguration(timeout, backoffDelay);
    }
}
