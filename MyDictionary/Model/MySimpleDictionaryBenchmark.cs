using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;

public class MySimpleDictionaryBenchmark
{
    private MySimpleDictionary<int, int>? dictionary;
    private int[]? keys;
    private int keyToSearch;

    [Params(1000, 10000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        dictionary = new MySimpleDictionary<int, int>();
        keys = new int[N];

        var random = new Random(42);
        for (int i = 0; i < N; i++)
        {
            keys[i] = random.Next();
            dictionary.Add(keys[i], i);
        }

        keyToSearch = keys[N / 2];
    }

    [Benchmark]
    public int GetExistingKey()
    {
        if (dictionary == null)
        {
            throw new InvalidOperationException("Dictionary is not initialized.");
        }

        return dictionary.Get(keyToSearch);
    }

    [Benchmark]
    public bool CheckContainsKey()
    {
        if (dictionary == null)
        {
            throw new InvalidOperationException("Dictionary is not initialized.");
        }
        return dictionary.ContainsKey(keyToSearch);
    }

    [Benchmark]
    public bool RemoveKey()
    {
        if (dictionary == null)
        {
            throw new InvalidOperationException("Dictionary is not initialized.");
        } 
        
        return dictionary.Remove(keyToSearch);
    }
}