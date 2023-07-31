using System.Collections;

namespace Jamarino.ValueEnumerable;

public struct EmptyOrEnumerable<T> : IEnumerable<T>
{
    private List<T> _items;
    
    public void Add(T item)
    {
        if (_items is null)
            _items = new List<T>();

        _items.Add(item);
    }

    public ValueEnumerator GetEnumerator()
    {
        return new ValueEnumerator(_items);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return GetEnumerator();
    }

    public struct ValueEnumerator : IEnumerator<T>
    {
        private readonly List<T> _items;
        private int _index;
        private int _itemCount;

        public ValueEnumerator(List<T> items)
        {
            _items = items;
            _itemCount = items?.Count ?? 0;
        }

        public T Current { get; set; }

        object IEnumerator.Current => Current!;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_index < _itemCount)
            {
                Current = _items[_index];
                _index++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
