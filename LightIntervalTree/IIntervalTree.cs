namespace LightIntervalTree
{
    public interface IIntervalTree<TKey,TValue>
    {
        void Add(TKey from, TKey to, TValue value);

        IEnumerable<TValue> Query(TKey key);
    }
}
