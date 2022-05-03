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

    [GlobalSetup]
    public void GlobalSetup()
    {
        const long RangeMin = 300_000_000_000;
        const long RangeMax = 700_000_000_000;
        var random = new Random(123123);

        _ranges = Enumerable.Range(0, 300_000)
            .Select(i => random.NextInt64(RangeMin / 1000, RangeMax / 1000))
            .Select(i => (i * 1000, (i * 1000) + (random.Next(1, 3) * 1000) - 1, random.Next(10_000)))
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
