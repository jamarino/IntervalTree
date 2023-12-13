using BenchmarkDotNet.Attributes;
using Extras;
using Jamarino.IntervalTree;

namespace Benchmark;

[MemoryDiagnoser]
public class QueryRangeBenchmarks
{
    private Dictionary<string, object> _treeCache = new();
    private Dictionary<string, IEnumerable<Interval<long, int>>> _dataCache = new();

    [Params("sparse", "medium", "dense")]
    public string DataType { get; set; } = string.Empty;

    [Params(250_000)]
    public int IntervalCount = 1;

    public object GetLoadedTree(string treeType, string dataType)
    {
        var key = $"{treeType}:{dataType}";
        if (_treeCache.ContainsKey(key))
        {
            return _treeCache[key];
        }

        if (treeType is "reference")
        {
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
        else
        {
            var tree = (IIntervalTree<long, int>)TreeFactory.CreateEmptyTreeRaw<long, int>(treeType);

            var data = _dataCache[dataType];

            foreach (var interval in data)
            {
                tree.Add(interval.From, interval.To, interval.Value);
            }
            tree.Query(0);

            _treeCache[key] = tree;
            return tree;
        }
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

    [Benchmark(OperationsPerInvoke = 10_000, Baseline = true)]
    public void QueryReference()
    {
        var tree = (IntervalTree.IntervalTree<long, int>)GetLoadedTree("reference", DataType);
        var range = 0;
        for (var i = 0; i < 10_000; i++)
        {
            range = range % 100 + 1;
            var results = tree.Query(i, i + range);
            var sum = 0;
            foreach (var result in results)
            {
                sum += result;
            }
        }
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void QueryLight()
    {
        var tree = (LightIntervalTree<long, int>)GetLoadedTree("light", DataType);
        var range = 0;
        for (var i = 0; i < 10_000; i++)
        {
            range = range % 100 + 1;
            var results = tree.Query(i, i + range);
            var sum = 0;
            foreach (var result in results)
            {
                sum += result;
            }
        }
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void QueryQuick()
    {
        var tree = (QuickIntervalTree<long, int>)GetLoadedTree("quick", DataType);
        var range = 0;
        for (var i = 0; i < 10_000; i++)
        {
            range = range % 100 + 1;
            var results = tree.Query(i, i + range);
            var sum = 0;
            foreach (var result in results)
            {
                sum += result;
            }
        }
    }
}
