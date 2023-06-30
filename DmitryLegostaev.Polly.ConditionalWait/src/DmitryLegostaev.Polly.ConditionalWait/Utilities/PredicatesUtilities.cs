namespace DmitryLegostaev.Polly.ConditionalWait.Utilities;

public static class PredicatesUtilities
{
    public static Func<T, bool> NegateFuncTBoolResult<T>(Func<T, bool> conditionPredicate)
    {
        return t => !conditionPredicate(t);
    }
}
