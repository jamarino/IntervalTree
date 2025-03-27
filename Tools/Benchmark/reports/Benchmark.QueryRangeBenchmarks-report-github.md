``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22631
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=8.0.204
  [Host]     : .NET 8.0.4 (8.0.424.16909), X64 RyuJIT
  Job-XISHJK : .NET 8.0.4 (8.0.424.16909), X64 RyuJIT

MaxIterationCount=20  MaxWarmupIterationCount=10  MinIterationCount=5  

```
|    Method | IntervalCount |        Mean |     Error |   StdDev |  Ratio | RatioSD |  Gen 0 |  Gen 1 | Allocated |
|---------- |-------------- |------------:|----------:|---------:|-------:|--------:|-------:|-------:|----------:|
| **Reference** |           **100** |    **323.2 ns** |   **5.93 ns** |  **2.63 ns** |   **1.00** |    **0.00** | **0.0771** |      **-** |   **1,298 B** |
|    Linear |           100 |    150.7 ns |   2.51 ns |  1.31 ns |   0.47 |    0.01 | 0.0215 |      - |     362 B |
|     Light |           100 |    164.2 ns |   2.27 ns |  0.59 ns |   0.51 |    0.01 | 0.0195 |      - |     328 B |
|     Quick |           100 |    167.6 ns |   2.99 ns |  0.78 ns |   0.52 |    0.00 | 0.0195 |      - |     328 B |
|           |               |             |           |          |        |         |        |        |           |
| **Reference** |        **100000** |    **576.0 ns** |  **10.81 ns** |  **2.81 ns** |   **1.00** |    **0.00** | **0.1689** | **0.0010** |   **2,827 B** |
|    Linear |        100000 | 69,067.5 ns | 278.96 ns | 72.45 ns | 119.90 |    0.53 |      - |      - |     362 B |
|     Light |        100000 |    231.0 ns |   4.20 ns |  0.65 ns |   0.40 |    0.00 | 0.0207 |      - |     352 B |
|     Quick |        100000 |    237.2 ns |   2.64 ns |  0.68 ns |   0.41 |    0.00 | 0.0207 |      - |     352 B |
