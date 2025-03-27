using BenchmarkDotNet.Attributes;
using Extras;
using Jamarino.IntervalTree;
using Microsoft.Diagnostics.Runtime.Interop;

namespace Benchmark;

[MemoryDiagnoser]
[MinIterationCount(5)]
[MaxIterationCount(20)]
[MaxWarmupCount(10)]
public class ConcurrencyBenchmarks
{
    private Interval<long, int>[] _data = [];
    private int _max = 0;
    private object? _cachedTree;

    [Params(100, /* 1_000, 10_000,*/ 100_000)]
    public int IntervalCount { get; set; } = 1;

    private T GetLoadedTree<T>()
        where T : class, new()
    {
        if (_cachedTree is not null)
            return (_cachedTree as T)!;

        var tree = new T();
        if (tree is IntervalTree.IntervalTree<long, int> rangeTree)
        {
            foreach (var item in _data)
                rangeTree.Add(item.From, item.To, item.Value);
        }
        else if (tree is IIntervalTree<long, int> intervalTree)
        {
            foreach (var item in _data)
                intervalTree.Add(item.From, item.To, item.Value);
        }

        _cachedTree = tree;
        return tree;
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _cachedTree = null;
        var random = new Random(123);
        _data = IntervalGenerator.GenerateLongInt(IntervalCount, random);
        var max = _data.Max(i => i.To);
        _max = max < int.MaxValue ? (int)max : int.MaxValue;
    }

    [Benchmark(OperationsPerInvoke = 10_000, Baseline = true)]
    public int Reference()
    {
        var tree = GetLoadedTree<IntervalTree.IntervalTree<long, int>>();
        var ret = 0;

        Parallel.For(
            0,
            10_000,
            new ParallelOptions { MaxDegreeOfParallelism = 16 },
            () => 0,
            (i, s, l) =>
            {
                var target = i % _max;
                var results = tree.Query(target);
                return results.Count();
            },
            (l) => ret = l);

        return ret;
    }

    [Benchmark(OperationsPerInvoke = 100_000)]
    public int Light()
    {
        var tree = GetLoadedTree<LightIntervalTree<long, int>>();
        var ret = 0;

        Parallel.For(
            0,
            100_000,
            new ParallelOptions { MaxDegreeOfParallelism = 16 },
            () => 0,
            (i, s, l) =>
            {
                var target = i % _max;
                var results = tree.Query(target);
                return results.Count();
            },
            (l) => ret = l);

        return ret;
    }

    [Benchmark(OperationsPerInvoke = 100_000)]
    public int Quick()
    {
        var tree = GetLoadedTree<QuickIntervalTree<long, int>>();
        var ret = 0;

        Parallel.For(
            0,
            100_000,
            new ParallelOptions { MaxDegreeOfParallelism = 16 },
            () => 0,
            (i, s, l) =>
            {
                var target = i % _max;
                var results = tree.Query(target);
                return results.Count();
            },
            (l) => ret = l);

        return ret;
    }
}
