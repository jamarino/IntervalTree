using BenchmarkDotNet.Attributes;
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
        
        _intervals = new Interval<long, int>[IntervalCount];
        var prev = rand.Next(0, 3);
        
        for (int i = 0; i < IntervalCount; i++)
        {
            prev += Math.Min(rand.Next(10), rand.Next(10));
            _intervals[i] = new(prev, prev + rand.Next(1, 10), i);
        }
        
        var end = _intervals[IntervalCount - 1].To;
        
        _queries = Enumerable.Range(0, QueryCount)
            .Select(_ => rand.NextInt64(end))
            .ToArray();

        Shuffle(_intervals, rand);
    }

    private void Shuffle<T>(T[] data, Random rand)
    {
        var n = data.Length;
        while (n > 1)
        {
            var k = rand.Next(n--);
            var tmp = data[n];
            data[n] = data[k];
            data[k] = tmp;
        }
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = _runs)]
    public void Reference()
    {
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
                var sum = results.Sum();
            }
        }
    }

    [Benchmark(OperationsPerInvoke = _runs)]
    public void Linear()
    {
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
                var sum = results.Sum();
            }
        }
    }

    [Benchmark(OperationsPerInvoke = _runs)]
    public void Light()
    {
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
                var sum = results.Sum();
            }
        }
    }

    [Benchmark(OperationsPerInvoke = _runs)]
    public void Quick()
    {
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
                var sum = results.Sum();
            }
        }
    }
}
