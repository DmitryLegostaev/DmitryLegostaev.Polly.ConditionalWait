using DmitryLegostaev.Polly.ConditionalWait.Configuration;
using Microsoft.Extensions.Logging;

namespace DmitryLegostaev.Polly.ConditionalWait;

public interface IConditionalWait
{
    public T WaitForPredicateAndGetResult<T>(Func<T> codeToExecute, Func<T, bool> conditionPredicate, 
        TimeSpan? timeout = null, TimeSpan? backoffDelay = null, 
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null);

    public T WaitForPredicateAndGetResult<T>(Func<T> codeToExecute, Func<T, bool> conditionPredicate,
        IConditionalWaitConfiguration waitConfiguration, 
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null);

    public T WaitForAndGetResult<T>(Func<T> codeToExecute, 
        TimeSpan? timeout = null, TimeSpan? backoffDelay = null,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null);

    public T WaitForAndGetResult<T>(Func<T> codeToExecute, 
        IConditionalWaitConfiguration waitConfiguration,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null);

    public void WaitForTrue(Func<bool> codeToExecute, 
        TimeSpan? timeout = null, TimeSpan? backoffDelay = null,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null);

    public void WaitForTrue(Func<bool> codeToExecute, 
        IConditionalWaitConfiguration waitConfiguration,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null, ILogger? logger = null);
}
