using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public interface IMySimpleDictionary<TKey, TValue>
    : IEnumerable<KeyValuePair<TKey, TValue>>
{
    void Add(TKey key, TValue value);
    bool TryAdd(TKey key, TValue value);
    TValue Get(TKey key);
    bool TryGetValue(TKey key, out TValue value);
    bool ContainsKey(TKey key);
    bool ContainsValue(TValue value);
    bool Remove(TKey key);
    void Clear();

    int Count { get; }
    TValue this[TKey key] { get; set; }
    IEnumerable<TKey> Keys { get; }
    IEnumerable<TValue> Values { get; }
    IEqualityComparer<TKey> Comparer { get; }
    IEnumerable<KeyValuePair<TKey, TValue>> GetAllItems();

    void CopyTo(KeyValuePair<TKey, TValue>[] array, int index);
    void Add(KeyValuePair<TKey, TValue> item);
    bool Contains(KeyValuePair<TKey, TValue> item);
    bool Remove(KeyValuePair<TKey, TValue> item);
}