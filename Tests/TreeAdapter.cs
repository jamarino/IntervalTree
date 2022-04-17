using IntervalTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class TreeAdapter<TKey, TValue> : IIntervalTree<TKey, TValue>
    {
        public TreeAdapter(LightIntervalTree.IIntervalTree<TKey, TValue> lightTree)
        {
            LightTree = lightTree;
        }

        public IEnumerable<TValue> Values => throw new NotImplementedException();

        public int Count => LightTree.Count;

        public LightIntervalTree.IIntervalTree<TKey, TValue> LightTree { get; }

        public void Add(TKey from, TKey to, TValue value)
        {
            LightTree.Add(from, to, value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<RangeValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> Query(TKey value)
        {
            return LightTree.Query(value);
        }

        public IEnumerable<TValue> Query(TKey from, TKey to)
        {
            throw new NotImplementedException();
        }

        public void Remove(TValue item)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<TValue> items)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
