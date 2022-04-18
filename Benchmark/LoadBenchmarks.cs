using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Extras;

namespace Benchmark
{
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    public class LoadBenchmarks
    {
        private List<(long, long, int)> _ranges = new();
        private List<long> _tests = new();
        private Dictionary<string, IntervalTree.IIntervalTree<long, int>> _preloadedTrees = new();

        private readonly string[] TreeTypes = { "original", "largesparse" };

        [Params("original", "largesparse")]
        public string TreeType { get; set; }

        public IntervalTree.IIntervalTree<long, int> GetEmptyTree(string type)
        {
            return type switch
            {
                "original" => new IntervalTree.IntervalTree<long, int>(),
                "largesparse" => new TreeAdapter<long, int>(new LightIntervalTree.LargeSparseIntervalTree<long, int>()),
                _ => throw new ArgumentException($"Unkown tree type: {type}", nameof(type))
            };
        }

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

            _tests = Enumerable.Range(0, 10_000)
                .Select(_ => random.NextInt64(RangeMin, RangeMax))
                .ToList();

            foreach (var type in TreeTypes)
            {
                var tree = GetEmptyTree(type);
                foreach (var range in _ranges)
                {
                    tree.Add(range.Item1, range.Item2, range.Item3);
                }
                tree.Query(0);
                _preloadedTrees.Add(type, tree);
            }
        }

        [Benchmark]
        public void Load()
        {
            var tree = GetEmptyTree(TreeType);
            foreach (var (from, to, val) in _ranges)
            {
                tree.Add(from, to, val);
            }
            tree.Query(0);
        }

        [Benchmark(OperationsPerInvoke = 10_000)]
        public void Query()
        {
            var tree = _preloadedTrees[TreeType];
            foreach (var test in _tests)
            {
                tree.Query(test);
            }
        }
    }
}
