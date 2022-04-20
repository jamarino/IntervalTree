using BenchmarkDotNet.Attributes;
using Extras;

namespace Benchmark;

public class QueryBenchmarks
{
    const int IntervalCount = 100_000;
    private Dictionary<string, IntervalTree.IIntervalTree<long, int>> _treeCache = new();
    private Dictionary<string, IEnumerable<Interval>> _dataCache = new();


    [Params("original", "largesparse", "balanced")]
    public string TreeType { get; set; } = string.Empty;

    [Params("sparse", "dense", "ascending", "descending")]
    public string DataType { get; set; } = string.Empty;

    public IntervalTree.IIntervalTree<long, int> GetLoadedTree(string treeType, string dataType)
    {
        var key = $"{treeType}:{dataType}";
        if (_treeCache.ContainsKey(key))
        {
            return _treeCache[key];
        }

        IntervalTree.IIntervalTree<long, int> tree = treeType switch
        {
            "original" => new IntervalTree.IntervalTree<long, int>(),
            "largesparse" => new TreeAdapter<long, int>(new LightIntervalTree.LargeSparseIntervalTree<long, int>()),
            "balanced" => new TreeAdapter<long, int>(new LightIntervalTree.LargeSparseBalancedIntervalTree<long, int>()),
            _ => throw new ArgumentException(nameof(treeType))
        };

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
        
        var sparse = Enumerable.Range(0, IntervalCount)
            .Select(_ =>
            {
                var start = random.Next(10*IntervalCount);
                return new Interval
                {
                    From = start,
                    To = start + random.Next(1, 20),
                };
            })
            .ToList();
        _dataCache["sparse"] = sparse;

        var dense = Enumerable.Range(0, IntervalCount)
            .Select(_ =>
            {
                var start = random.Next(10*IntervalCount);
                return new Interval
                {
                    From = start,
                    To = start + random.Next(5, 200),
                };
            })
            .ToList();
        _dataCache["dense"] = dense;

        var ascending = sparse.OrderBy(i => i.From).ToList();
        _dataCache["ascending"] = ascending;

        var descending = sparse.OrderByDescending(i => i.From).ToList();
        _dataCache["descending"] = ascending;
    }

    [Benchmark(OperationsPerInvoke = IntervalCount)]
    public void Query()
    {
        var tree = GetLoadedTree(TreeType, DataType);
        for (var i = 0; i < IntervalCount; i++)
        {
            tree.Query(10*i);
        }
    }
}
