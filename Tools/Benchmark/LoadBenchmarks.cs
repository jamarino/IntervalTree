using BenchmarkDotNet.Attributes;
using Extras;

namespace Benchmark;

[MemoryDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class LoadBenchmarks
{
    private List<(long, long, int)> _ranges = new();

    public string[] TreeTypes => TreeFactory.TreeTypes;

    [ParamsSource(nameof(TreeTypes))]
    public string TreeType { get; set; } = string.Empty;

    [Params(250_000)]
    public int IntervalCount = 1;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var random = new Random(123);

        var max = 10 * IntervalCount;
        var maxIntervalSize = 2 * max / IntervalCount;

        _ranges = Enumerable.Range(0, IntervalCount)
            .Select(i => random.NextInt64(0, max))
            .Select(i => (i, i + random.Next(1, maxIntervalSize), random.Next(10_000)))
            .ToList();
    }

    [Benchmark]
    public void Load()
    {
        var tree = TreeFactory.CreateEmptyTree<long, int>(TreeType);
        foreach (var (from, to, val) in _ranges)
        {
            tree.Add(from, to, val);
        }
        tree.Query(0);
    }
}
