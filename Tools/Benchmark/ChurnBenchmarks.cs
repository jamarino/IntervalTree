using BenchmarkDotNet.Attributes;
using Extras;
using Jamarino.IntervalTree;

namespace Benchmark;

[MemoryDiagnoser]
[IterationTime(200)]
[MinIterationCount(10)]
[MaxIterationCount(20)]
[MinWarmupCount(5)]
[MaxWarmupCount(10)]
public class ChurnBenchmarks
{
    private Interval<long, int>[] _intervals = [];
    private long[] _queries = [];
    private const int _runs = 100;

    [Params(10, 50, 250, 1_000)]
    public int IntervalCount = 1;

    [Params(1, 10, 100)]
    public int QueryCount = 1;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rand = new Random(1337);
        
        _intervals = IntervalGenerator.GenerateLongInt(IntervalCount, rand);
                
        var max = _intervals.Max(i => i.To);
        _queries = Enumerable.Range(0, QueryCount)
            .Select(_ => rand.NextInt64(max))
            .ToArray();
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = _runs)]
    public int Reference()
    {
        var ret = 0;
        var tree = new IntervalTree.IntervalTree<long, int>();

        for (var run = 0; run < _runs; run++)
        {
            tree.Clear();

            // load data
            for (var i = 0; i < IntervalCount; i ++)
            {
                var interval = _intervals[i];
                tree.Add(interval.From, interval.To, interval.Value);
            }

            // query
            for (var i = 0; i < QueryCount; i++)
            {
                var results = tree.Query(_queries[i]);
                ret = results.Count();
            }
        }

        return ret;
    }

    [Benchmark(OperationsPerInvoke = _runs)]
    public int Linear()
    {
        var ret = 0;
        var tree = new LinearIntervalTree<long, int>(IntervalCount);

        for (var run = 0; run < _runs; run++)
        {
            tree.Clear();

            // load data
            for (var i = 0; i < IntervalCount; i++)
            {
                var interval = _intervals[i];
                tree.Add(interval.From, interval.To, interval.Value);
            }

            // query
            for (var i = 0; i < QueryCount; i++)
            {
                var results = tree.Query(_queries[i]);
                ret = results.Count();
            }
        }

        return ret;
    }

    [Benchmark(OperationsPerInvoke = _runs)]
    public int Light()
    {
        var ret = 0;
        var tree = new LightIntervalTree<long, int>(IntervalCount);

        for (var run = 0; run < _runs; run++)
        {
            tree.Clear();

            // load data
            for (var i = 0; i < IntervalCount; i++)
            {
                var interval = _intervals[i];
                tree.Add(interval.From, interval.To, interval.Value);
            }

            // query
            for (var i = 0; i < QueryCount; i++)
            {
                var results = tree.Query(_queries[i]);
                ret = results.Count();
            }
        }

        return ret;
    }

    [Benchmark(OperationsPerInvoke = _runs)]
    public int Quick()
    {
        var ret = 0;
        var tree = new QuickIntervalTree<long, int>(IntervalCount);

        for (var run = 0; run < _runs; run++)
        {
            tree.Clear();

            // load data
            for (var i = 0; i < IntervalCount; i++)
            {
                var interval = _intervals[i];
                tree.Add(interval.From, interval.To, interval.Value);
            }

            // query
            for (var i = 0; i < QueryCount; i++)
            {
                var results = tree.Query(_queries[i]);
                ret = results.Count();
            }
        }

        return ret;
    }
}
