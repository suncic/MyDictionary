using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public interface IMySimpleDictionary<TKey, TValue>
{
    bool Add(TKey key, TValue value);
    TValue Get(TKey key);
    bool ContainsKey(TKey key);
    bool ContainsValue(TValue value);
    int Count { get; }
    bool Remove(TKey key);
    void Clear();
    TValue this[TKey key] { get; set; }
    IEnumerable<KeyValuePair<TKey, TValue>> GetAllItems();
    ICollection<TKey> Keys { get; }
    ICollection<TValue> Values { get; }
}