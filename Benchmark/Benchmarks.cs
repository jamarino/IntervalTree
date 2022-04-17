using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        private List<(long,long,int)> _ranges = new();
        private List<long> _tests = new();
        private IntervalTree.IntervalTree<long, int> _treeV1 = new();
        private LightIntervalTree.LargeSparseIntervalTree<long, int> _treeV2 = new();

        [ParamsSource(nameof(PrepareEmptyTrees))]
        public IntervalTree.IIntervalTree<long, int> EmptyTree { get; set; }

        private IEnumerable<IntervalTree.IIntervalTree<long, int>> PrepareEmptyTrees()
        {
            yield return new IntervalTree.IntervalTree<long, int>();
            yield return new TreeAdapter<long, int>(new LightIntervalTree.LargeSparseIntervalTree<long, int>());
            yield return new TreeAdapter<long, int>(new LightIntervalTree.LargeDenseIntervalTree<long, int>());
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            const long RangeMin = 300_000_000_000;
            const long RangeMax = 700_000_000_000;
            var random = new Random(123123);
            _ranges = Enumerable.Range(0, 300_000)
                .Select(i => random.NextInt64(RangeMin/1000, RangeMax/1000))
                .Select(i => (i * 1000, (i * 1000) + (random.Next(1, 3) * 1000) - 1, random.Next(10_000)))
                .ToList();
            _tests = Enumerable.Range(0, 1000)
                .Select(_ => random.NextInt64(RangeMin, RangeMax))
                .ToList();

            foreach (var range in _ranges)
            {
                _treeV1.Add(range.Item1, range.Item2, range.Item3);
                _treeV2.Add(range.Item1, range.Item2, range.Item3);
            }

            _treeV1.Query(0);
            _treeV2.Optimize();
        }


        [Benchmark]
        public void LoadV1()
        {
            var tree = new IntervalTree.IntervalTree<long,int>();
            foreach (var (from, to, val) in _ranges)
            {
                tree.Add(from, to, val);
            }
            tree.Query(1);
        }

        [Benchmark]
        public void LoadV2()
        {
            var tree = new LightIntervalTree.LargeSparseIntervalTree<long, int>();
            foreach (var (from, to, val) in _ranges)
            {
                tree.Add(from, to, val);
            }
            tree.Optimize();
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void QueryV1()
        {
            foreach (var t in _tests)
            {
                _treeV1.Query(t);
            }
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void QueryV2()
        {
            foreach (var t in _tests)
            {
                _treeV2.Query(t);
            }
        }
    }
}
