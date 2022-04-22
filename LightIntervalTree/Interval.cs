namespace LightIntervalTree;

public readonly record struct Interval<TKey, TValue>
{
    public Interval(TKey from, TKey to, TValue value)
    {
        From = from;
        To = to;
        Value = value;
    }

    public readonly TKey From { get; }
    public readonly TKey To { get; }
    public readonly TValue Value { get; }
};
