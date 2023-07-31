using System.Collections;

namespace Jamarino.ValueEnumerable;

public struct ValueEnumerable<T> : IEnumerable<T>
{
    private List<T> _items;
    private T _first;
    private bool _isFirstItemSet;
    
    public void Add(T item)
    {
        if (!_isFirstItemSet)
        {
            _first = item;
            _isFirstItemSet = true;
        }
        else
        {
            if (_items is null)
                _items = new List<T>() { _first };

            _items.Add(item);
        }
    }

    public ValueEnumerator GetEnumerator()
    {
        return new ValueEnumerator(this);
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
        private readonly ValueEnumerable<T> enumerable;
        private int _moved;

        public ValueEnumerator(ValueEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
        }

        public T Current { get; set; }

        object IEnumerator.Current => Current!;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (enumerable._items is null)
            {
                if (_moved > 0) return false;
                if (!enumerable._isFirstItemSet) return false;
                
                Current = enumerable._first;
                _moved++;
                return true;
            }
            else
            {
                if (_moved >= enumerable._items.Count)
                    return false;

                Current = enumerable._items[_moved];
                _moved++;
                return true;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
