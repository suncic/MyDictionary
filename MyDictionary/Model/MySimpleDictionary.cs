using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class MySimpleDictionary<TKey, TValue> : IMySimpleDictionary<TKey, TValue>
{
    private const double LOAD_FACTOR = 0.75;
    private int count = 0;
    private IEqualityComparer<TKey> comparer = EqualityComparer<TKey>.Default;
    private IEqualityComparer<TValue> valueComparer = EqualityComparer<TValue>.Default;

    public class Node
    {
        public TKey key;
        public TValue value;
        public Node next;

        public Node(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
            this.next = null;
        }
    }

    private Node[] table;
    public IEqualityComparer<TKey> Comparer => comparer;
    public MySimpleDictionary() : this(101, EqualityComparer<TKey>.Default) { }

    public MySimpleDictionary(int capacity) : this(capacity, EqualityComparer<TKey>.Default)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity must be greater than zero.");
        }
    }

    public MySimpleDictionary(int size, IEqualityComparer<TKey> comparer)
    {
        if (size <= 0)
        {
            throw new ArgumentException("Size must be greater than zero.", nameof(size));
        }
        if (comparer == null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }

        table = new Node[size];
        this.comparer = comparer;
    }

    public MySimpleDictionary(IDictionary<TKey, TValue> other, IEqualityComparer<TKey> comparer) 
        : this(other?.Count ?? 0, comparer)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        foreach (var pair in other)
        {
            Add(pair.Key, pair.Value);
        }
    }

    public MySimpleDictionary(IDictionary<TKey, TValue> other) 
        : this(other, EqualityComparer<TKey>.Default)
    {
        
    }

    private int Hash(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key), "Key cannot be null.");
        }

        return (key.GetHashCode() & 0x7FFFFFFF) % table.Length;
    }

    private bool SearchColissionChain(TKey Key, int hashValue, out Node current, out Node previous)
    {
        current = table[hashValue];
        previous = null;

        while (current != null)
        {
            if (comparer.Equals(current.key, Key))
            {
                return true;
            }

            previous = current;
            current = current.next;
        }

        return false;
    }

    public void Add(TKey key, TValue value)
    {
        if ((double)count / table.Length > LOAD_FACTOR)
        {
            Resize();
        }

        int hashValue = Hash(key);

        if (SearchColissionChain(key, hashValue, out var current, out _))
        {
            throw new ArgumentException("Key already exists in the dictionary.", nameof(key));
        }

        Node newNode = new Node(key, value);
        newNode.next = table[hashValue];
        table[hashValue] = newNode;
        count++;
    }

    public bool TryAdd(TKey key, TValue value)
    {
        if ((double)count / table.Length > LOAD_FACTOR)
        {
            Resize();
        }

        int hashValue = Hash(key);

        if (SearchColissionChain(key, hashValue, out var current, out _))
        {
            return false;
        }

        Node newNode = new Node(key, value);
        newNode.next = table[hashValue];
        table[hashValue] = newNode;
        count++;
        return true;
    }

    private static int GetNextPrime(int n)
    {
        if (n <= 2)
            return 2;

        if (n % 2 == 0)
            n++;

        while (!IsPrime(n))
        {
            n += 2;
        }
        return n;
    }

    private static bool IsPrime(int n)
    {
        if (n < 2) return false;
        if (n == 2) return true;
        if (n % 2 == 0) return false;

        int sqrt = (int)Math.Sqrt(n);
        for (int i = 3; i <= sqrt; i += 2)
        {
            if (n % i == 0)
                return false;
        }
        return true;
    }

    private void Resize()
    {
        int newSize = GetNextPrime(table.Length * 2);
        var newTable = new Node[newSize];
        foreach (var pair in this)
        {
            int hash = (pair.Key.GetHashCode() & 0x7FFFFFFF) % newTable.Length;
            Node newNode = new Node(pair.Key, pair.Value);
            newNode.next = newTable[hash];
            newTable[hash] = newNode;
        }
        table = newTable;
    }

    public TValue Get(TKey key)
    {
        int hashValue = Hash(key);
        if (SearchColissionChain(key, hashValue, out var current, out var previous))
        {
            if (current != table[hashValue])
            {
                previous.next = current.next;
                current.next = table[hashValue];
                table[hashValue] = current;
            }
            return current.value;
        }

        throw new KeyNotFoundException("Key not found: " + key);
    }


    public bool TryGetValue(TKey key, out TValue value)
    {
        int hashValue = Hash(key);
        if (SearchColissionChain(key, hashValue, out var current, out _))
        {
            value = current.value;
            return true;
        }
        value = default(TValue);
        return false;
    }
    public bool ContainsKey(TKey key) => SearchColissionChain(key, Hash(key), out _, out _);

    public bool ContainsValue(TValue value)
    {
        foreach (var pair in this)
        {
            if (valueComparer.Equals(pair.Value, value))
            {
                return true;
            }
        }
        return false;
    }

    public int Count => count;

    public bool Remove(TKey key)
    {
        int hashValue = Hash(key);
        if (SearchColissionChain(key, hashValue, out var current, out var previous))
        {
            if (current == table[hashValue])
            {
                table[hashValue] = current.next;
            }
            else
            {
                previous.next = current.next;
            }

            count--;
            return true;
        }
        return false;
    }

    public void Clear()
    {
        Array.Clear(table, 0, table.Length);
        count = 0;
    }

    public TValue this[TKey key]
    {
        get => Get(key);
        set
        {
            int hashValue = Hash(key);
            if (SearchColissionChain(key, hashValue, out var current, out _))
            {
                current.value = value;
            }
            else
            {
                Add(key, value);
            }
        }
    }

    public IEnumerable<KeyValuePair<TKey, TValue>> GetAllItems()
    {
        for (int i = 0; i < table.Length; i++)
        {
            Node current = table[i];
            while (current != null)
            {
                yield return new KeyValuePair<TKey, TValue>(current.key, current.value);
                current = current.next;
            }
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return GetAllItems().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array), "Array cannot be null.");
        }
        if (index < 0 || index >= array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }
        if (array.Length - index < Count)
        {
            throw new ArgumentException("The number of elements in the source dictionary is greater than the available space from index to the end of the destination array.");
        }

        foreach(var pair in this)
        {
            array[index++] = pair;
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return GetAllItems().Any(pair =>
            comparer.Equals(pair.Key, item.Key) &&
                valueComparer.Equals(pair.Value, item.Value));
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (Contains(item))
        {
            return Remove(item.Key);
        }
        return false;
    }

    public IEnumerable<TKey> Keys
    {
        get
        {
            foreach (var pair in this)
            {
                yield return pair.Key;
            }
        }
    }

    public IEnumerable<TValue> Values
    {
        get
        {
            foreach (var pair in this)
            {
                yield return pair.Value;
            }
        }
    }
}