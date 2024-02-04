using BenchmarkDotNet.Attributes;
using Extras;
using Jamarino.IntervalTree;

namespace Benchmark;

[MemoryDiagnoser]
[MinIterationCount(5)]
[MaxIterationCount(10)]
[MinWarmupCount(3)]
[MaxWarmupCount(5)]
[IterationTime(200)]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class LoadBenchmarks
{
    private const int _runs = 8;
    private Interval<long, int>[] _data = [];

    [Params(100, 1_000, 10_000, 100_000)]
    public int IntervalCount = 1;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var random = new Random(123);
        _data = IntervalGenerator.GenerateLongInt(IntervalCount, random);
    }

    [Benchmark(OperationsPerInvoke = _runs, Baseline = true)]
    public void Reference()
    {
        for (int run = 0; run < _runs; run++)
        {
            var tree = new IntervalTree.IntervalTree<long, int>();

            for (var i = 0; i < IntervalCount; i++)
            {
                var interval = _data[i];
                tree.Add(interval.From, interval.To, interval.Value);
            }

            tree.Query(0); // force build
        }
    }

    [Benchmark(OperationsPerInvoke = _runs)]
    public void LinearHint()
    {
        for (int run = 0; run < _runs; run++)
        {
            var tree = new LinearIntervalTree<long, int>(IntervalCount);

            for (var i = 0; i < IntervalCount; i++)
            {
                var interval = _data[i];
                tree.Add(interval.From, interval.To, interval.Value);
            }

            // no build required
        }
    }

    [Benchmark(OperationsPerInvoke = _runs)]
    public void Linear()
    {
        for (int run = 0; run < _runs; run++)
        {
            var tree = new LinearIntervalTree<long, int>();

            for (var i = 0; i < IntervalCount; i++)
            {
                var interval = _data[i];
                tree.Add(interval.From, interval.To, interval.Value);
            }

            // no build required
        }
    }

    [Benchmark(OperationsPerInvoke = _runs)]
    public void LightHint()
    {
        for (int run = 0; run < _runs; run++)
        {
            var tree = new LightIntervalTree<long, int>(IntervalCount);

            for (var i = 0; i < IntervalCount; i++)
            {
                var interval = _data[i];
                tree.Add(interval.From, interval.To, interval.Value);
            }

            tree.Build();
        }
    }

    [Benchmark(OperationsPerInvoke = _runs)]
    public void Light()
    {
        for (int run = 0; run < _runs; run++)
        {
            var tree = new LightIntervalTree<long, int>();

            for (var i = 0; i < IntervalCount; i++)
            {
                var interval = _data[i];
                tree.Add(interval.From, interval.To, interval.Value);
            }

            tree.Build();
        }
    }

    [Benchmark(OperationsPerInvoke = _runs)]
    public void QuickHint()
    {
        for (int run = 0; run < _runs; run++)
        {
            var tree = new QuickIntervalTree<long, int>(IntervalCount);

            for (var i = 0; i < IntervalCount; i++)
            {
                var interval = _data[i];
                tree.Add(interval.From, interval.To, interval.Value);
            }

            tree.Build();
        }
    }

    [Benchmark(OperationsPerInvoke = _runs)]
    public void Quick()
    {
        for (int run = 0; run < _runs; run++)
        {
            var tree = new QuickIntervalTree<long, int>();

            for (var i = 0; i < IntervalCount; i++)
            {
                var interval = _data[i];
                tree.Add(interval.From, interval.To, interval.Value);
            }

            tree.Build();
        }
    }
}
