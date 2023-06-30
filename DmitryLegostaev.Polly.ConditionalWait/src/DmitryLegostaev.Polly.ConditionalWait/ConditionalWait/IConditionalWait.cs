﻿using DmitryLegostaev.Polly.ConditionalWait.Configuration;

namespace DmitryLegostaev.Polly.ConditionalWait.ConditionalWait;

public interface IConditionalWait
{
    public T WaitForPredicateAndGetResult<T>(Func<T> codeToExecute, Func<T, bool> conditionPredicate, TimeSpan? timeout = null,
        TimeSpan? backoffDelay = null, IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null);

    public T WaitForPredicateAndGetResult<T>(Func<T> codeToExecute, Func<T, bool> conditionPredicate,
        IConditionalWaitConfiguration waitConfiguration, IList<Type>? exceptionsToIgnore = null, string? failReason = null,
        string? codePurpose = null);

    public T WaitForAndGetResult<T>(Func<T> codeToExecute, TimeSpan? timeout = null, TimeSpan? backoffDelay = null,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null);

    public T WaitForAndGetResult<T>(Func<T> codeToExecute, IConditionalWaitConfiguration waitConfiguration,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null);

    public void WaitForTrue(Func<bool> codeToExecute, TimeSpan? timeout = null, TimeSpan? backoffDelay = null,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null);

    public void WaitForTrue(Func<bool> codeToExecute, IConditionalWaitConfiguration waitConfiguration,
        IList<Type>? exceptionsToIgnore = null, string? failReason = null, string? codePurpose = null);
}