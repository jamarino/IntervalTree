namespace Jamarino.IntervalTree;

public readonly record struct Interval<TKey, TValue> : IComparable<Interval<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    public Interval(TKey from, TKey to, TValue value)
    {
        if (from.CompareTo(to) > 0)
        {
            throw new ArgumentOutOfRangeException($"'{nameof(from)}' must be less than or equal to '{nameof(to)}'");
        }
        
        From = from;
        To = to;
        Value = value;
    }

    public readonly TKey From { get; }
    public readonly TKey To { get; }
    public readonly TValue Value { get; }

    public int CompareTo(Interval<TKey, TValue> other)
    {
        var compareFrom = From.CompareTo(other.From);
        if (compareFrom != 0)
            return compareFrom;
        return To.CompareTo(other.To);
    }
}
