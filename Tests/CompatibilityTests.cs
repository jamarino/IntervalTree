using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class CompatibilityTests
    {
        public static readonly string[] TreeTypes = { "original", "largesparse", "largedense" };

        public IntervalTree.IIntervalTree<long, int> CreateEmptyTree(string type) => type switch
        {
            "original" => new IntervalTree.IntervalTree<long, int>(),
            "largesparse" => new TreeAdapter<long, int>(new LightIntervalTree.LargeSparseIntervalTree<long, int>()),
            "largedense" => new TreeAdapter<long, int>(new LightIntervalTree.LargeDenseIntervalTree<long, int>()),
            _ => throw new ArgumentException($"Unkown tree type: {type}", nameof(type))
        };

        public class AnEmptyTree : CompatibilityTests
        {
            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueried_Returns0Results(string treeType)
            {
                var tree = CreateEmptyTree(treeType);

                var results = tree.Query(0);

                Assert.That(results.Count(), Is.EqualTo(0));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenAddedTo_DoesNotExplode(string treeType)
            {
                var tree = CreateEmptyTree(treeType);

                Assert.DoesNotThrow(() => tree.Add(10, 20, 1));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenAddedTo_IncreasesCountTo1(string treeType)
            {
                var tree = CreateEmptyTree(treeType);

                tree.Add(10, 20, 1);

                Assert.That(tree.Count, Is.EqualTo(1));
            }
        }

        public class ATreeWithOneInterval : CompatibilityTests
        {
            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedInInterval_ReturnsMatch(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 19, 1);

                var results = tree.Query(15);

                Assert.That(results.Count(), Is.EqualTo(1));
                Assert.That(results.First(), Is.EqualTo(1));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedAtFromValue_ReturnsMatch(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 19, 1);

                var results = tree.Query(10);

                Assert.That(results.Count(), Is.EqualTo(1));
                Assert.That(results.First(), Is.EqualTo(1));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedAtToValue_ReturnsMatch(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 19, 1);

                var results = tree.Query(19);

                Assert.That(results.Count(), Is.EqualTo(1));
                Assert.That(results.First(), Is.EqualTo(1));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedBeforeFromValue_Returns0Matches(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 19, 1);

                var results = tree.Query(9);

                Assert.That(results.Count(), Is.EqualTo(0));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedAfterToValue_Returns0Matches(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 19, 1);

                var results = tree.Query(20);

                Assert.That(results.Count(), Is.EqualTo(0));
            }
        }
    
        public class ATreeWithTwoDisjointIntervals : CompatibilityTests
        {
            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void HasACountOfTwo(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 19, 1);
                tree.Add(50, 59, 2);

                Assert.That(tree.Count, Is.EqualTo(2));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedInFirstInterval_ReturnsValueOfFirstInterval(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 19, 1);
                tree.Add(50, 59, 2);

                var results = tree.Query(15);

                Assert.That(results.Count, Is.EqualTo(1));
                Assert.That(results.First(), Is.EqualTo(1));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedInSecondInterval_ReturnsValueOfSecondInterval(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 19, 1);
                tree.Add(50, 59, 2);

                var results = tree.Query(55);

                Assert.That(results.Count, Is.EqualTo(1));
                Assert.That(results.First(), Is.EqualTo(2));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedBeforeFirstInterval_Returns0Results(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 19, 1);
                tree.Add(50, 59, 2);

                var results = tree.Query(5);

                Assert.That(results.Count, Is.EqualTo(0));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedBetweenIntervals_Returns0Results(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 19, 1);
                tree.Add(50, 59, 2);

                var results = tree.Query(35);

                Assert.That(results.Count, Is.EqualTo(0));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedAfterSecondInterval_Returns0Results(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 19, 1);
                tree.Add(50, 59, 2);

                var results = tree.Query(75);

                Assert.That(results.Count, Is.EqualTo(0));
            }
        }

        public class ATreeWithTwoPatiallyOverlappingIntervals : CompatibilityTests
        {
            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void HasACountOfTwo(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 29, 1);
                tree.Add(20, 39, 2);

                Assert.That(tree.Count, Is.EqualTo(2));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedInFirstInterval_ReturnsValueOfFirstInterval(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 29, 1);
                tree.Add(20, 39, 2);

                var results = tree.Query(15);

                Assert.That(results.Count, Is.EqualTo(1));
                Assert.That(results.First(), Is.EqualTo(1));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedInSecondInterval_ReturnsValueOfSecondInterval(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 29, 1);
                tree.Add(20, 39, 2);

                var results = tree.Query(35);

                Assert.That(results.Count, Is.EqualTo(1));
                Assert.That(results.First(), Is.EqualTo(2));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedBeforeFirstInterval_Returns0Results(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 29, 1);
                tree.Add(20, 39, 2);

                var results = tree.Query(5);

                Assert.That(results.Count, Is.EqualTo(0));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedInOverlap_ReturnsBothIntervalValues(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 29, 1);
                tree.Add(20, 39, 2);

                var results = tree.Query(25);

                Assert.That(results.Count, Is.EqualTo(2));
                Assert.That(results.Contains(1), Is.True);
                Assert.That(results.Contains(2), Is.True);
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedAfterSecondInterval_Returns0Results(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 29, 1);
                tree.Add(20, 39, 2);

                var results = tree.Query(75);

                Assert.That(results.Count, Is.EqualTo(0));
            }
        }

        public class ATreeWithTwoCompletelyOverlappingIntervals : CompatibilityTests
        {
            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void HasACountOfTwo(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 49, 1);
                tree.Add(20, 39, 2);

                Assert.That(tree.Count, Is.EqualTo(2));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedInFirstIntervalBeforeSecondInterval_ReturnsValueOfFirstInterval(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 49, 1);
                tree.Add(20, 39, 2);

                var results = tree.Query(15);

                Assert.That(results.Count, Is.EqualTo(1));
                Assert.That(results.First(), Is.EqualTo(1));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedInFirstIntervalAfterSecondInterval_ReturnsValueOfFirstInterval(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 49, 1);
                tree.Add(20, 39, 2);

                var results = tree.Query(45);

                Assert.That(results.Count, Is.EqualTo(1));
                Assert.That(results.First(), Is.EqualTo(1));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedBeforeFirstInterval_Returns0Results(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 49, 1);
                tree.Add(20, 39, 2);

                var results = tree.Query(5);

                Assert.That(results.Count, Is.EqualTo(0));
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedInOverlap_ReturnsBothIntervalValues(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 49, 1);
                tree.Add(20, 39, 2);

                var results = tree.Query(25);

                Assert.That(results.Count, Is.EqualTo(2));
                Assert.That(results.Contains(1), Is.True);
                Assert.That(results.Contains(2), Is.True);
            }

            [Test]
            [TestCaseSource(nameof(TreeTypes))]
            public void WhenQueriedAfterFirstInterval_Returns0Results(string treeType)
            {
                var tree = CreateEmptyTree(treeType);
                tree.Add(10, 49, 1);
                tree.Add(20, 39, 2);

                var results = tree.Query(75);

                Assert.That(results.Count, Is.EqualTo(0));
            }
        }

        public class OutputTests : CompatibilityTests
        {
            private record struct Interval
            {
                public long From { get; set; }
                public long To { get; set; }
                public int Value { get; set; }
            }

            [Test]
            //[TestCase(123)]
            //[TestCase(234)]
            //[TestCase(345)]
            //[TestCase(456)]
            public void CompareResults([Range(1, 25)] int seed)
            {
                var random = new Random(seed);

                var intervals = Enumerable.Range(0, 16)
                    .Select(_ => random.NextInt64(0, 100))
                    .Select((from, i) => new Interval { From = from, To = from + random.Next(1, 10), Value = i })
                    .ToList();

                var originalTree = CreateEmptyTree("original");
                var trees = TreeTypes
                    .Where(t => t is not "original")
                    .Select(t => CreateEmptyTree(t))
                    .ToList();

                foreach (var interval in intervals)
                {
                    originalTree.Add(interval.From, interval.To, interval.Value);
                    foreach (var tree in trees)
                    {
                        tree.Add(interval.From, interval.To, interval.Value); 
                    }
                }
                
                for (var i = 0; i < 10_100; i++)
                {
                    var reference = originalTree.Query(i);
                    var results = trees
                        .Select(t => t.Query(i))
                        .Select(r => new HashSet<int>(r))
                        .ToList();

                    for (var j = 0; j < trees.Count; j++)
                    {
                        Assert.That(results[j], Is.EquivalentTo(reference), $"Result mismatch for Query({i})"); 
                    }
                }
            }
        }
    }
}
