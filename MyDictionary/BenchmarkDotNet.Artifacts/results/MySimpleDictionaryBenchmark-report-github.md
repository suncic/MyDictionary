```

BenchmarkDotNet v0.15.2, macOS Monterey 12.7.6 (21H1320) [Darwin 21.6.0]
Intel Core i5-5257U CPU 2.70GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.304
  [Host]     : .NET 8.0.19 (8.0.1925.36514), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.19 (8.0.1925.36514), X64 RyuJIT AVX2


```
| Method           | N     | Mean      | Error    | StdDev   | Median    |
|----------------- |------ |----------:|---------:|---------:|----------:|
| **GetExistingKey**   | **1000**  |  **21.80 ns** | **0.992 ns** | **2.847 ns** |  **20.53 ns** |
| CheckContainsKey | 1000  |  28.71 ns | 1.647 ns | 4.480 ns |  26.61 ns |
| RemoveKey        | 1000  |  19.20 ns | 0.078 ns | 0.065 ns |  19.18 ns |
| **GetExistingKey**   | **10000** |  **22.42 ns** | **1.105 ns** | **3.026 ns** |  **21.49 ns** |
| CheckContainsKey | 10000 | 140.10 ns | 0.838 ns | 1.354 ns | 139.60 ns |
| RemoveKey        | 10000 | 232.38 ns | 0.522 ns | 0.436 ns | 232.30 ns |
