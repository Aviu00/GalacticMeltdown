using System;
using System.Collections;
using System.Collections.Generic;

namespace GalacticMeltdown.Collections;

public sealed class OrderedSet<T> : ICollection<T>
{
    private readonly IDictionary<T, LinkedListNode<T>> _dictionary;
    private readonly LinkedList<T> _linkedList;

    public OrderedSet()
        : this(EqualityComparer<T>.Default)
    {
    }

    public OrderedSet(IEqualityComparer<T> comparer)
    {
        _dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
        _linkedList = new LinkedList<T>();
    }

    public int Count => _dictionary.Count;

    public bool IsReadOnly => _dictionary.IsReadOnly;

    public T Last => _linkedList.Last is null ? default : _linkedList.Last.Value;

    void ICollection<T>.Add(T item)
    {
        Add(item);
    }

    public void Clear()
    {
        _linkedList.Clear();
        _dictionary.Clear();
    }

    public bool Remove(T item)
    {
        if (item is null || !_dictionary.TryGetValue(item, out LinkedListNode<T> node)) return false;
        _dictionary.Remove(item);
        _linkedList.Remove(node);
        return true;
    }

    public T Pop()
    {
        if (_linkedList.Last is null) throw new Exception();
        T item = _linkedList.Last.Value;
        Remove(item);
        return item;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _linkedList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Contains(T item)
    {
        return _dictionary.ContainsKey(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _linkedList.CopyTo(array, arrayIndex);
    }

    public bool Add(T item)
    {
        if (_dictionary.ContainsKey(item)) return false;
        LinkedListNode<T> node = _linkedList.AddLast(item);
        _dictionary.Add(item, node);
        return true;
    }
}