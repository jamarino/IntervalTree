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
    private readonly List<AugmentedInterval> _intervals = new();
    private bool _isBuilt = false;
    private int _treeHeight = 0;

    public int Count => _intervals.Count;

    public IEnumerable<TValue> Values => _intervals.Select(i => i.Value);

    /// <summary>
    /// Add an interval to the tree. Note that the tree will be rebuilt on the next query.
    /// </summary>
    public void Add(TKey from, TKey to, TValue value)
    {
        _intervals.Add(new AugmentedInterval(from, to, to, value));
        _isBuilt = false;
    }

    public IEnumerator<Interval<TKey, TValue>> GetEnumerator() => 
        _intervals.Select(i => new Interval<TKey, TValue>(i.From, i.To, i.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Find the values associated with all intervals containing the provided target value.
    /// Note that the tree will first be built if required.
    /// </summary>
    /// <param name="target">The value to test against stored intervals</param>
    /// <returns>Values associated with matching intervals</returns>
    public IEnumerable<TValue> Query(TKey target)
    {
        if (_isBuilt is false)
            Build();

        if (_intervals.Count == 0)
            return Enumerable.Empty<TValue>();

        List<TValue>? results = null;

        Span<int> stack = stackalloc int[2 * _treeHeight];
        stack[0] = 0;
        stack[1] = _intervals.Count - 1;
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
                var center = (min + max + 1) / 2;
                var interval = _intervals[center];

                var compareMax = target.CompareTo(interval.Max);
                if (compareMax > 0) continue; // target larger than Max, bail

                // search left
                stack[++stackIndex] = min;
                stack[++stackIndex] = center - 1;

                // check current node
                var compareFrom = target.CompareTo(interval.From);
                var compareTo = target.CompareTo(interval.To);

                if (compareFrom >= 0 && compareTo <= 0)
                {
                    results ??= new List<TValue>();
                    results.Add(interval.Value);
                }

                if (compareFrom < 0) continue; // target smaller than From, bail

                // search right
                stack[++stackIndex] = center + 1;
                stack[++stackIndex] = max;
            }
        }

        return results is null ? Enumerable.Empty<TValue>() : results;
    }

    /// <summary>
    /// Build the underlying tree structure.
    /// This operation is NOT thread safe.
    /// A build is automatically performed, if needed, on the first query after altering the tree.
    /// This operation takes O(n log n) time. 
    /// </summary>
    public void Build()
    {
        if (_intervals.Count is 0)
        {
            _treeHeight = 0;
            _isBuilt = true;
            return;
        }

        // order intervals
        _intervals.Sort();
        _treeHeight = (int)Math.Log(_intervals.Count, 2) + 1;

        UpdateMaxRec(0, _intervals.Count - 1);

        _isBuilt = true;

        TKey UpdateMaxRec(int min, int max)
        {
            var center = (min + max + 1) / 2;

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

                var left = UpdateMaxRec(min, center - 1);
                var right = center < max ?
                    UpdateMaxRec(center + 1, max) :
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
