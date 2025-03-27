using Benchmark;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<LoadBenchmarks>();
BenchmarkRunner.Run<QueryBenchmarks>();
BenchmarkRunner.Run<ConcurrencyBenchmarks>();
BenchmarkRunner.Run<QueryRangeBenchmarks>();
BenchmarkRunner.Run<ChurnBenchmarks>();
