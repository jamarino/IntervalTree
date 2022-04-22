namespace LightIntervalTree;

public interface IIntervalTree<TKey,TValue> : IEnumerable<Interval<TKey, TValue>>
{
    int Count { get; }

    IEnumerable<TValue> Values { get; }

    void Add(TKey from, TKey to, TValue value);

    IEnumerable<TValue> Query(TKey key);
}
