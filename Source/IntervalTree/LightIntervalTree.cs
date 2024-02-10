using System.Collections;

namespace Jamarino.IntervalTree;

/// <summary>
/// Light-weight interval tree implementation, based on an augmented interval tree.
/// </summary>
/// <typeparam name="TKey">Type used to specify the start and end of each intervals</typeparam>
/// <typeparam name="TValue">Type of the value associated with each interval</typeparam>
public class LightIntervalTree<TKey, TValue> : IIntervalTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    private readonly object _buildLock = new();
    private AugmentedInterval[] _intervals;
    private int _count = 0;
    private bool _isBuilt = false;
    private int _treeHeight = 0;

    /// <inheritdoc cref="LightIntervalTree{TKey, TValue}"/>
    public LightIntervalTree() : this(null) { }

    /// <inheritdoc cref="LightIntervalTree{TKey, TValue}"/>
    public LightIntervalTree(int? initialCapacity = null)
    {
        _intervals = new AugmentedInterval[initialCapacity ?? 16];
    }

    public int Count => _count;

    public IEnumerable<TValue> Values =>
        _intervals.Take(_count).Select(i => i.Value);

    public void Add(TKey from, TKey to, TValue value)
    {
        if (_intervals.Length == _count)
        {
            var newArray = new AugmentedInterval[2 * _count];
            Array.Copy(_intervals, newArray, _count);
            _intervals = newArray;
        }

        _intervals[_count++] = new(from, to, to, value);
        _isBuilt = false;
    }

    public IEnumerator<Interval<TKey, TValue>> GetEnumerator() => 
        _intervals.Take(_count).Select(i => new Interval<TKey, TValue>(i.From, i.To, i.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<TValue> Query(TKey target)
    {
        if (_isBuilt is false)
            Build();

        if (_count == 0)
            return Enumerable.Empty<TValue>();

        List<TValue>? results = null;

        Span<int> stack = stackalloc int[2 * _treeHeight];
        stack[0] = 0;
        stack[1] = _count - 1;
        var stackIndex = 1;

        while (stackIndex > 0)
        {
            var max = stack[stackIndex--];
            var min = stack[stackIndex--];

            var span = max - min;
            if (span < 6) // At small subtree sizes a linear scan is faster
            {
                for (var i = min; i <= max; i++)
                {
                    var interval = _intervals[i];

                    var compareFrom = target.CompareTo(interval.From);
                    if (compareFrom < 0)
                        break;

                    var compareTo = target.CompareTo(interval.To);
                    if (compareTo > 0)
                        continue;

                    results ??= new List<TValue>();
                    results.Add(interval.Value);
                }
            }
            else
            {
                var center = min + (max - min + 1) / 2;
                var interval = _intervals[center];

                var compareMax = target.CompareTo(interval.Max);
                if (compareMax > 0) continue; // target larger than Max, bail
                
                // search left
                stack[++stackIndex] = min;
                stack[++stackIndex] = center - 1;

                // check current node
                var compareFrom = target.CompareTo(interval.From);

                if (compareFrom < 0) continue; // target smaller than From, bail
                else
                {
                    var compareTo = target.CompareTo(interval.To);
                    if (compareTo <= 0)
                    {
                        results ??= new List<TValue>();
                        results.Add(interval.Value);
                    }
                }

                // search right
                stack[++stackIndex] = center + 1;
                stack[++stackIndex] = max;
            }
        }

        return results is null ? Enumerable.Empty<TValue>() : results;
    }

    public IEnumerable<TValue> Query(TKey low, TKey high)
    {
        if (high.CompareTo(low) < 0)
            throw new ArgumentException("Argument 'high' must not be smaller than argument 'low'", nameof(high));

        if (_isBuilt is false)
            Build();

        if (_count == 0)
            return Enumerable.Empty<TValue>();

        List<TValue>? results = null;

        Span<int> stack = stackalloc int[2 * _treeHeight];
        stack[0] = 0;
        stack[1] = _count - 1;
        var stackIndex = 1;

        while (stackIndex > 0)
        {
            var max = stack[stackIndex--];
            var min = stack[stackIndex--];

            var span = max - min;
            if (span < 6) // At small subtree sizes a linear scan is faster
            {
                for (var i = min; i <= max; i++)
                {
                    var interval = _intervals[i];

                    var compareFrom = high.CompareTo(interval.From);
                    if (compareFrom < 0)
                        break;

                    var compareTo = low.CompareTo(interval.To);
                    if (compareTo > 0)
                        continue;

                    results ??= new List<TValue>();
                    results.Add(interval.Value);
                }
            }
            else
            {
                var center = (min + max + 1) / 2;
                var interval = _intervals[center];

                var compareMax = low.CompareTo(interval.Max);
                if (compareMax > 0) continue; // target larger than Max, bail

                // search left
                stack[++stackIndex] = min;
                stack[++stackIndex] = center - 1;

                // check current node
                var compareFrom = high.CompareTo(interval.From);

                if (compareFrom < 0) continue; // target smaller than From, bail
                else
                {
                    var compareTo = low.CompareTo(interval.To);
                    if (compareTo <= 0)
                    {
                        results ??= new List<TValue>();
                        results.Add(interval.Value);
                    }
                }

                // search right
                stack[++stackIndex] = center + 1;
                stack[++stackIndex] = max;
            }
        }

        return results is null ? Enumerable.Empty<TValue>() : results;
    }

    /// <summary>
    /// Build the underlying tree structure.
    /// A build is automatically performed, if needed, on the first query after altering the tree.
    /// </summary>
    public void Build()
    {
        lock (_buildLock)
        {
            if (_isBuilt) return;

            if (_count is 0)
            {
                _treeHeight = 0;
                _isBuilt = true;
                return;
            }

            // order intervals
            Array.Sort(_intervals, 0, _count);
            _treeHeight = (int)Math.Log(_count, 2) + 1;

            UpdateMaxRec(0, _count - 1, 0);

            _isBuilt = true;
        }

        TKey UpdateMaxRec(int min, int max, int recursionLevel)
        {
            if (recursionLevel++ > 100)
                throw new Exception($"Excessive recursion detected, aborting to prevent stack overflow. Please check thread safety.");

            var center = min + (max - min + 1) / 2;

            var interval = _intervals[center];

            if (max - min <= 0)
            {
                // set max to 'To'
                _intervals[center] = interval with
                {
                    Max = interval.To
                };
                return interval.To;
            }
            else
            {
                // find max between children and own 'To'
                var maxValue = interval.To;

                var left = UpdateMaxRec(min, center - 1, recursionLevel);
                var right = center < max ?
                    UpdateMaxRec(center + 1, max, recursionLevel) :
                    maxValue;

                if (left.CompareTo(maxValue) > 0)
                    maxValue = left;
                if (right.CompareTo(maxValue) > 0)
                    maxValue = right;

                // update max
                _intervals[center] = interval with
                {
                    Max = maxValue
                };
                return maxValue;
            }
        }
    }

    public void Remove(TValue value)
    {
        var i = 0;
        while (i < _count)
        {
            var interval = _intervals[i];
            if (Equals(interval.Value, value))
            {
                _count--;
                _intervals[i] = _intervals[_count];
                _isBuilt = false;
            }
            else
            {
                i++;
            }
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
        _isBuilt = false;
    }

    private record struct AugmentedInterval : IComparable<AugmentedInterval>
    {
        public AugmentedInterval(TKey from, TKey to, TKey max, TValue value)
        {
            From = from;
            To = to;
            Max = max;
            Value = value;
        }

        public TKey From;
        public TKey To;
        public TKey Max;
        public TValue Value;

        public int CompareTo(AugmentedInterval other)
        {
            var fromComparison = From.CompareTo(other.From);
            if (fromComparison != 0)
                return fromComparison;
            return To.CompareTo(other.To);
        }
    }
}
