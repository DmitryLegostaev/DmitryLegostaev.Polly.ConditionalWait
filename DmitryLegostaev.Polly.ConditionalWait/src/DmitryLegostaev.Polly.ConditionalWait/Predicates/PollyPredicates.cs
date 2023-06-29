namespace DmitryLegostaev.Polly.ConditionalWait.Predicates;

public static class PollyPredicates
{
    public static Func<T, bool> IsNotNullPredicate<T>() => t => t is not null;
    public static readonly Func<bool, bool> IsTruePredicate = t => t;
}
