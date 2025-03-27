``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22631
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=8.0.204
  [Host]     : .NET 8.0.4 (8.0.424.16909), X64 RyuJIT
  Job-XISHJK : .NET 8.0.4 (8.0.424.16909), X64 RyuJIT

MaxIterationCount=20  MaxWarmupIterationCount=10  MinIterationCount=5  

```
|    Method | IntervalCount |      Mean |     Error |    StdDev | Ratio |  Gen 0 |  Gen 1 | Allocated |
|---------- |-------------- |----------:|----------:|----------:|------:|-------:|-------:|----------:|
| **Reference** |           **100** | **20.184 ns** | **0.2681 ns** | **0.0696 ns** |  **1.00** | **0.0208** | **0.0007** |     **348 B** |
|     Light |           100 |  8.514 ns | 0.1380 ns | 0.0613 ns |  0.42 | 0.0051 | 0.0001 |      85 B |
|     Quick |           100 |  7.686 ns | 0.0952 ns | 0.0147 ns |  0.38 | 0.0051 | 0.0001 |      85 B |
|           |               |           |           |           |       |        |        |           |
| **Reference** |        **100000** | **62.412 ns** | **0.5309 ns** | **0.2357 ns** |  **1.00** | **0.0654** | **0.0032** |   **1,093 B** |
|     Light |        100000 | 15.255 ns | 0.1852 ns | 0.0481 ns |  0.24 | 0.0051 | 0.0001 |      86 B |
|     Quick |        100000 | 13.610 ns | 0.0982 ns | 0.0152 ns |  0.22 | 0.0051 | 0.0001 |      86 B |
