using Benchmark;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<LoadBenchmarks>();
BenchmarkRunner.Run<QueryBenchmarks>();
BenchmarkRunner.Run<QueryRangeBenchmarks>();
BenchmarkRunner.Run<ChurnBenchmarks>();
