using BenchmarkDotNet.Attributes;
using Extras;
using Jamarino.IntervalTree;

namespace Benchmark;

[MemoryDiagnoser]
public class QueryRangeBenchmarks
{
    private Dictionary<string, IntervalTree.IIntervalTree<long, int>> _treeCache = new();
    private Dictionary<string, IEnumerable<Interval<long, int>>> _dataCache = new();

    public string[] TreeTypes => TreeFactory.TreeTypes;

    [ParamsSource(nameof(TreeTypes))]
    public string TreeType { get; set; } = string.Empty;

    [Params("sparse", "medium", "dense")]
    public string DataType { get; set; } = string.Empty;

    [Params(250_000)]
    public int IntervalCount = 1;

    public IntervalTree.IIntervalTree<long, int> GetLoadedTree(string treeType, string dataType)
    {
        var key = $"{treeType}:{dataType}";
        if (_treeCache.ContainsKey(key))
        {
            return _treeCache[key];
        }

        var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);

        var data = _dataCache[dataType];

        foreach (var interval in data)
        {
            tree.Add(interval.From, interval.To, interval.Value);
        }
        tree.Query(0);

        _treeCache[key] = tree;
        return tree;
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        var random = new Random(123);
        
        // approx 20% coverage
        var sparse = Enumerable.Range(0, IntervalCount)
            .Select(_ =>
            {
                var start = random.Next(25*IntervalCount);
                return new Interval<long, int>(
                    start,
                    start + random.Next(1, 10),
                    1);
            })
            .ToList();
        _dataCache["sparse"] = sparse;

        // approx 100% coverage
        var medium = Enumerable.Range(0, IntervalCount)
            .Select(_ =>
            {
                var start = random.Next(10*IntervalCount);
                return new Interval<long, int>(
                    start,
                    start + random.Next(1, 20),
                    1);
            })
            .ToList();
        _dataCache["medium"] = medium;

        // approx 500% coverage
        var dense = Enumerable.Range(0, IntervalCount)
            .Select(_ =>
            {
                var start = random.Next(10 * IntervalCount);
                return new Interval<long, int>(
                    start,
                    start + random.Next(1, 100),
                    1);
            })
            .ToList();
        _dataCache["dense"] = dense;
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Query()
    {
        var tree = GetLoadedTree(TreeType, DataType);
        var range = 0;
        for (var i = 0; i < 10_000; i++)
        {
            range = range % 100 + 1;
            tree.Query(i, i + range);
        }
    }
}
