using System.Collections;

namespace Jamarino.IntervalTree;

/// <summary>
/// Simple implementation using full scans of an unordered array, not actually a tree. O(n) queries.
/// Does not require an expensive build process, making it the fastest options when (#intervals * #queries) &lt; 1000
/// </summary>
/// <typeparam name="TKey">Type used to specify the start and end of each intervals</typeparam>
/// <typeparam name="TValue">Type of the value associated with each interval</typeparam>
public class LinearIntervalTree<TKey, TValue> : IIntervalTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    private Interval<TKey, TValue>[] _intervals;
    private int _count;

    /// <inheritdoc cref="LinearIntervalTree{TKey, TValue}"/>
    public LinearIntervalTree() : this(null) { }

    /// <inheritdoc cref="LinearIntervalTree{TKey, TValue}"/>
    public LinearIntervalTree(int? initialCapacity = null)
    {
        _intervals = new Interval<TKey, TValue>[initialCapacity ?? 8];
    }

    public int Count => _count;

    public IEnumerable<TValue> Values => 
        _intervals.Take(_count).Select(i => i.Value);

    public void Add(TKey from, TKey to, TValue value)
    {
        if (_intervals.Length == _count)
        {
            var newArray = new Interval<TKey, TValue>[2 * _intervals.Length];
            Array.Copy(_intervals, newArray, _intervals.Length);
            _intervals = newArray;
        }

        _intervals[_count++] = new(from, to, value);
    }

    public IEnumerator<Interval<TKey, TValue>> GetEnumerator() => 
        _intervals.Take(_count).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<TValue> Query(TKey target)
    {
        if (_count is 0)
            return Enumerable.Empty<TValue>();

        List<TValue>? results = null;

        for (var i = 0; i < _count; i++)
        {
            var interval = _intervals[i];

            var compareFrom = target.CompareTo(interval.From);
            if (compareFrom < 0)
                continue;

            var compareTo = target.CompareTo(interval.To);
            if (compareTo > 0)
                continue;

            results ??= new List<TValue>();
            results.Add(interval.Value);
        }

        return results is null ? Enumerable.Empty<TValue>() : results;
    }

    public IEnumerable<TValue> Query(TKey low, TKey high)
    {
        if (high.CompareTo(low) < 0)
            throw new ArgumentException("Argument 'high' must not be smaller than argument 'low'", nameof(high));

        if (_count == 0)
            return Enumerable.Empty<TValue>();

        List<TValue>? results = null;

        for (var i = 0; i < _count; i++)
        {
            var interval = _intervals[i];

            var compareFrom = high.CompareTo(interval.From);
            if (compareFrom < 0)
                continue;

            var compareTo = low.CompareTo(interval.To);
            if (compareTo > 0)
                continue;

            results ??= new List<TValue>();
            results.Add(interval.Value);
        }

        return results is null ? Enumerable.Empty<TValue>() : results;
    }

    public void Remove(TValue value)
    {
        var i = 0;
        while (i < _count)
        {
            var interval = _intervals[i];
            if (Equals(interval.Value, value))
                _intervals[i] = _intervals[--_count];
            else
                i++;
        }
    }

    public void Remove(IEnumerable<TValue> values)
    {
        foreach (var val in values)
            Remove(val);
    }

    public void Clear()
    {
        _count = 0;
    }
}
