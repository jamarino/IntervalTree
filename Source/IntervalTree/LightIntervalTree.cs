using System.Collections;

namespace Jamarino.IntervalTree;

public class LightIntervalTree<TKey, TValue> : IIntervalTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    private readonly List<OrderedInterval> _intervals = new();
    private bool _isBuilt = false;
    private int _treeHeight = 0;
    private int _treeSizeFull = 0;

    public int Count => _intervals.Count;

    public IEnumerable<TValue> Values => _intervals.Select(i => i.Value);

    public void Add(TKey from, TKey to, TValue value)
    {
        _intervals.Add(new OrderedInterval(from, to, to, value));
        _isBuilt = false;
    }

    public IEnumerator<Interval<TKey, TValue>> GetEnumerator() => 
        _intervals.Select(i => new Interval<TKey, TValue>(i.From, i.To, i.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<TValue> Query(TKey target)
    {
        if (_isBuilt is false)
            Build();

        if (_treeSizeFull is 0)
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

    public void Build()
    {
        if (_intervals.Count is 0)
        {
            _treeHeight = 0;
            _treeSizeFull = 0;
            _isBuilt = true;
            return;
        }

        // order intervals
        _intervals.Sort();
        _treeHeight = (int)Math.Log(_intervals.Count, 2) + 1;
        _treeSizeFull = (1 << _treeHeight) - 1;

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

    private record struct OrderedInterval : IComparable<OrderedInterval>
    {
        public OrderedInterval(TKey from, TKey to, TKey max, TValue value)
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

        public int CompareTo(OrderedInterval other)
        {
            var fromComparison = From.CompareTo(other.From);
            if (fromComparison != 0)
                return fromComparison;
            return To.CompareTo(other.To);
        }
    }
}
