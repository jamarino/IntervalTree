using BenchmarkDotNet.Attributes;
using Extras;
using Jamarino.IntervalTree;

namespace Benchmark;

[MemoryDiagnoser]
[MinIterationCount(5)]
[MaxIterationCount(20)]
[MaxWarmupCount(10)]
public class QueryBenchmarks
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

    [Benchmark(OperationsPerInvoke = 1_000, Baseline = true)]
    public void Reference()
    {
        var tree = GetLoadedTree<IntervalTree.IntervalTree<long, int>>();
        for (var i = 0; i < 1_000; i++)
        {
            var results = tree.Query(i % _max);
            _ = results.Count();
        }
    }

    [Benchmark(OperationsPerInvoke = 100)]
    public int Linear()
    {
        var tree = GetLoadedTree<LinearIntervalTree<long, int>>();
        var ret = 0;

        for (var i = 0; i < 10_000; i++)
        {
            var results = tree.Query(i % _max);
            ret = results.Count();
        }

        return ret;
    }

    [Benchmark(OperationsPerInvoke = 100)]
    public int LinearFind()
    {
        var tree = GetLoadedTree<LinearIntervalTree<long, int>>();
        var results = new List<int>();
        var ret = 0;

        for (var i = 0; i < 100; i++)
        {
            results.Clear();
            var target = i % _max;
            tree.Find(
                target,
                target,
                results,
                static (intv, s) => { s.Add(intv.Value); return s; });
            ret = results.Count;
        }

        return ret;
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public int Light()
    {
        var tree = GetLoadedTree<LightIntervalTree<long, int>>();
        var ret = 0;

        for (var i = 0; i < 10_000; i++)
        {
            var results = tree.Query(i % _max);
            ret = results.Count();
        }

        return ret;
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public int LightFind()
    {
        var tree = GetLoadedTree<LightIntervalTree<long, int>>();
        var results = new List<int>();
        var ret = 0;

        for (var i = 0; i < 10_000; i++)
        {
            results.Clear();
            var target = i % _max;
            tree.Find(
                target,
                target,
                results,
                static (intv, s) => { s.Add(intv.Value); return s; });
            ret = results.Count;
        }

        return ret;
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public int Quick()
    {
        var tree = GetLoadedTree<QuickIntervalTree<long, int>>();
        var ret = 0;

        for (var i = 0; i < 10_000; i++)
        {
            var results = tree.Query(i % _max);
            ret = results.Count();
        }

        return ret;
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public int QuickFind()
    {
        var tree = GetLoadedTree<QuickIntervalTree<long, int>>();
        var results = new List<int>();
        var ret = 0;

        for (var i = 0; i < 10_000; i++)
        {
            results.Clear();
            var target = i % _max;
            tree.Find(
                target,
                target,
                results,
                static (intv, s) => { s.Add(intv.Value); return s; });
            ret = results.Count;
        }

        return ret;
    }
}
