using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace GalacticMeltdown.Collections;

public class ObservableList<T> : IList<T>, INotifyCollectionChanged
{
    private List<T> _list = new();

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    int ICollection<T>.Count => _list.Count;
    public bool IsReadOnly => false;

    public T this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    public void Add(T item)
    {
        _list.Add(item);
        CollectionChanged?.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T> {item}, new List<T>()));
    }

    public void Clear()
    {
        List<T> prev = _list;
        _list = new List<T>();
        CollectionChanged?.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<T>(), prev));
    }

    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    public void Insert(int index, T item)
    {
        _list.Insert(index, item);
        CollectionChanged?.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T> {item}, new List<T>()));
    }

    public bool Remove(T item)
    {
        bool removed = _list.Remove(item);
        if (removed)
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(),
                    new List<T> {item}));
        return removed;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public IEnumerator GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ((IEnumerable<T>) _list).GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public void RemoveAt(int index)
    {
        if (!(index >= 0 && index < _list.Count)) throw new ArgumentOutOfRangeException();
        T item = _list[index];
        _list.RemoveAt(index);
        CollectionChanged?.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(),
                new List<T> {item}));
    }

    public void AddRange(IEnumerable<T> collection)
    {
        List<T> data = new(collection);
        _list.AddRange(data);
        CollectionChanged?.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, data, new List<T>()));
    }

    public void RemoveAll(Predicate<T> predicate)
    {
        List<T> data = _list.FindAll(predicate);
        _list.RemoveAll(predicate);
        CollectionChanged?.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<T>(), data));
    }

    public IEnumerable<T> DistinctBy<TKey>(Func<T, TKey> selector)
    {
        return _list.DistinctBy(selector);
    }

    public IEnumerable<T> Where(Func<T, bool> predicate)
    {
        return _list.Where(predicate);
    }

    public T First(Func<T, bool> predicate)
    {
        return _list.First(predicate);
    }

#nullable enable
    public T? FirstOrDefault(Func<T, bool> predicate)
    {
        return _list.FirstOrDefault(predicate);
    }
#nullable disable

    public int Count(Func<T, bool> predicate)
    {
        return _list.Count(predicate);
    }

    public bool All(Func<T, bool> predicate)
    {
        return _list.All(predicate);
    }
}