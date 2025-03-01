namespace Jamarino.IntervalTree;

public interface IIntervalTree<TKey, TValue> : IEnumerable<Interval<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    /// <summary>
    /// Number of stored intervals.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// All values associated with the stored intervals.
    /// </summary>
    IEnumerable<TValue> Values { get; }

    /// <summary>
    /// Add an interval to the tree. Note that the tree will be rebuilt on the next query.
    /// </summary>
    void Add(TKey from, TKey to, TValue value);

    /// <summary>
    /// Find the values associated with all intervals overlapping with the provided target key.
    /// Note that the tree will first be built if required.
    /// </summary>
    /// <param name="target">The key to test against stored intervals</param>
    /// <returns>Values associated with matching intervals</returns>
    IEnumerable<TValue> Query(TKey target);

    /// <summary>
    /// Find the values associated with all intervals overlapping the provided range.
    /// Note that the tree will first be built if required.
    /// </summary>
    /// <param name="target">The range to test against stored intervals</param>
    /// <returns>Values associated with matching intervals</returns>
    IEnumerable<TValue> Query(TKey low, TKey high);

    /// <summary>
    /// Remove all intervals with matching associated value.
    /// </summary>
    /// <param name="value">Value to look for</param>
    void Remove(TValue value);

    /// <summary>
    /// Remove all intervals with a value matching one of the provided values.
    /// </summary>
    /// <param name="value">Values to look for</param>
    void Remove(IEnumerable<TValue> values);

    /// <summary>
    /// Remove all values where the value matches the provided predicate. State is passed to the predicate.
    /// </summary>
    /// <param name="predicate">Predicate to test values against</param>
    /// <param name="state">State to pass to the predicate</param>
    void RemoveAll<TState>(Func<Interval<TKey, TValue>, TState, bool> predicate, TState state);

    /// <summary>
    /// Clear all data from tree. Allows for reusing of trees, instead of allocating a new ones.
    /// </summary>
    void Clear();
}
