using System.Collections;
using System.Collections.Generic;

public class CircularBuffer<T> : IEnumerable<T>
{
    private readonly T[] _data;
    private int _index = 0;
    private int _count = 0;

    public CircularBuffer(int capacity) => _data = new T[capacity];
    public int Count => _count;

    public void PushBack(T item)
    {
        _data[_index] = item;
        _index = (_index + 1) % _data.Length;
        _count = (_count < _data.Length) ? _count + 1 : _data.Length;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
            yield return _data[(_index - _count + i + _data.Length) % _data.Length];
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}