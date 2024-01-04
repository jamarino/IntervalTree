using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IntervalTree;

namespace Extras;

public class TreeAdapter<TKey, TValue> : IIntervalTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    public TreeAdapter(Jamarino.IntervalTree.IIntervalTree<TKey, TValue> lightTree)
    {
        LightTree = lightTree;
    }

    public IEnumerable<TValue> Values => LightTree.Values;

    public int Count => LightTree.Count;

    public Jamarino.IntervalTree.IIntervalTree<TKey, TValue> LightTree { get; }

    public void Add(TKey from, TKey to, TValue value) => LightTree.Add(from, to, value);

    public void Clear() => LightTree.Clear();

    public IEnumerator<RangeValuePair<TKey, TValue>> GetEnumerator() =>
        LightTree
            .Select(i => new RangeValuePair<TKey, TValue>(i.From, i.To, i.Value))
            .GetEnumerator();

    public IEnumerable<TValue> Query(TKey value)
        => LightTree.Query(value);

    public IEnumerable<TValue> Query(TKey from, TKey to)
        => LightTree.Query(from, to);

    public void Remove(TValue item)
    {
        throw new NotImplementedException();
    }

    public void Remove(IEnumerable<TValue> items)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
