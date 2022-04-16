using Benchmark;
using NUnit.Framework;
using LightIntervalTree;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Tests
{
    public class Tests
    {
        private List<(long, long, int)> _ranges = new();
        private List<(long, int[])> _tests = new();

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            const long RangeMin = 300_000_000_000;
            const long RangeMax = 700_000_000_000;
            var random = new Random(123123);
            _ranges = Enumerable.Range(0, 300_000)
                .Select(i => random.NextInt64(RangeMin / 1000, RangeMax / 1000))
                .Select(i => (i * 1000, (i * 1000) + (random.Next(1, 3) * 1000) - 1, random.Next(10_000)))
                .ToList();
            _tests = Enumerable.Range(0, 1000)
                .Select(_ => random.NextInt64(RangeMin, RangeMax))
                .Select(v => (v, _ranges.Where(r => r.Item1 <= v && v <= r.Item2).Select(r => r.Item3).ToArray()))
                .ToList();
        }

        [Test]
        public void TestAdd()
        {
            var tree = new LightIntervalTree.LargeSparseIntervalTree<long, int>();
            Assert.That(tree.TreeDepth, Is.EqualTo(0));
            tree.Add(40, 50, 1); // root
            Assert.That(tree.TreeDepth, Is.EqualTo(1));
            tree.Add(20, 30, 2); // left
            Assert.That(tree.TreeDepth, Is.EqualTo(2));
            tree.Add(70, 80, 3); // right
            Assert.That(tree.TreeDepth, Is.EqualTo(2));
            tree.Add(35, 55, 1); // root
            Assert.That(tree.TreeDepth, Is.EqualTo(2));
            tree.Add(15, 16, 4); // left-left
            Assert.That(tree.TreeDepth, Is.EqualTo(3));
            tree.Add(35, 36, 5); // left-right
            Assert.That(tree.TreeDepth, Is.EqualTo(3));
            tree.Add(85, 86, 7); // right-right
            Assert.That(tree.TreeDepth, Is.EqualTo(3));
            tree.Add(65, 66, 6); // right-left
            Assert.That(tree.TreeDepth, Is.EqualTo(3));

            Assert.That(tree.NodeCount, Is.EqualTo(7));
            Assert.That(tree.Query(45).Count(), Is.EqualTo(2));
            tree.Optimize();
        }

        [Test]
        public void TestQuery()
        {
            var tree = new LightIntervalTree.LargeSparseIntervalTree<long, int>();

            foreach (var range in _ranges)
            {
                tree.Add(range.Item1, range.Item2, range.Item3);
            }

            foreach (var test in _tests)
            {
                Assert.That(tree.Query(test.Item1), Is.EqualTo(test.Item2));
            }
        }

        [Test]
        public void TestOptimizeRight()
        {
            var tree = new LightIntervalTree.LargeSparseIntervalTree<long, int>();

            tree.Add(10, 19, 1);
            tree.Add(20, 29, 1);
            tree.Add(30, 39, 1);
            tree.Add(40, 49, 1);
            tree.Add(50, 59, 1);
            tree.Add(60, 69, 1);

            //tree.Optimize();
            tree.OptimizeRecursive();

            Assert.That(tree.TreeDepth, Is.EqualTo(3));
        }

        [Test]
        public void TestOptimizeLeft()
        {
            var tree = new LightIntervalTree.LargeSparseIntervalTree<long, int>();

            tree.Add(60, 69, 1);
            tree.Add(50, 59, 1);
            tree.Add(40, 49, 1);
            tree.Add(30, 39, 1);
            tree.Add(20, 29, 1);
            tree.Add(10, 19, 1);

            //tree.Optimize();
            tree.OptimizeRecursive();

            Assert.That(tree.TreeDepth, Is.EqualTo(3));
        }
    }
}