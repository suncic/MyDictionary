using System;
using System.Collections;
using System.Collections.Generic;

class MySimpleDictionary<TKey, TValue> : IMySimpleDictionary<TKey, TValue>
{
    private static int SIZE = 101;
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
    public MySimpleDictionary() : this(SIZE) { }

    public MySimpleDictionary(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity must be greater than zero.");
        }

        table = new Node[capacity];
    }

    public MySimpleDictionary(IEqualityComparer<TKey> comparer) : this(SIZE)
    {
        if (comparer == null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }

        this.comparer = comparer;
    }

    public MySimpleDictionary(IDictionary<TKey, TValue> other) : this(SIZE)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        foreach (var pair in other)
        {
            this.Add(pair.Key, pair.Value);
        }
    }

    private int Hash(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key), "Key cannot be null.");
        }

        return Math.Abs(key.GetHashCode() % table.Length);
    }

    private Node[] SearchColissionChain(TKey Key, int hashValue)
    {
        Node current = table[hashValue];
        Node previous = null;

        while (current != null)
        {
            if (comparer.Equals(current.key, Key))
            {
                Node[] n = new Node[2];
                n[0] = current;
                n[1] = previous;
                return n;
            }

            previous = current;
            current = current.next;
        }

        return null;
    }

    public bool Add(TKey key, TValue value)
    {
        int hashValue = Hash(key);

        if (SearchColissionChain(key, hashValue) != null)
        {
            return false;
        }

        Node newNode = new Node(key, value);
        newNode.next = table[hashValue];
        table[hashValue] = newNode;
        count++;
        return true;
    }

    public TValue Get(TKey key)
    {
        int hashValue = Hash(key);

        Node[] n = SearchColissionChain(key, hashValue);
        if (n == null)
        {
            return default(TValue);
        }

        // LRU strategija
        if (n[0] != table[hashValue])
        {
            n[1].next = n[0].next;
            n[0].next = table[hashValue];
            table[hashValue] = n[0];
        }

        return n[0].value;
    }
    public bool ContainsKey(TKey key)
    {
        return SearchColissionChain(key, Hash(key)) != null;
    }

    public bool ContainsValue(TValue value)
    {
        for (int i = 0; i < table.Length; i++)
        {
            Node current = table[i];
            while (current != null)
            {
                if (valueComparer.Equals(current.value, value))
                {
                    return true;
                }

                current = current.next;
            }

        }

        return false;
    }

    public int Count
    {
        get { return count; }
    }

    public bool Remove(TKey key)
    {
        int hashValue = Hash(key);

        Node[] n = SearchColissionChain(key, hashValue);
        if (n == null)
        {
            return false;
        }

        if (n[0] == table[hashValue])
        {
            table[hashValue] = table[hashValue].next;
        }
        else
        {
            n[1].next = n[0].next;
        }
        count--;

        return true;
    }

    public void Clear()
    {
        for (int i = 0; i < table.Length; i++)
        {
            table[i] = null;
        }
        count = 0;
    }

    private void Modify(TKey key, TValue value)
    {
        int hashValue = Hash(key);

        Node[] n = SearchColissionChain(key, hashValue);
        if (n[0] != null)
        {
            n[0].value = value;
        }
        else
        {
            Add(key, value);
        }
    }

    public TValue this[TKey key]
    {
        get { return Get(key); }
        set { Modify(key, value); }
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

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>();
            for (int i = 0; i < table.Length; i++)
            {
                Node current = table[i];
                while (current != null)
                {
                    keys.Add(current.key);
                    current = current.next;
                }
            }
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new HashSet<TValue>();
            for (int i = 0; i < table.Length; i++)
            {
                Node current = table[i];
                while (current != null)
                {
                    values.Add(current.value);
                    current = current.next;
                }
            }

            return values.ToList();
        }
    }
}