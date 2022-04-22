using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Extras;

namespace Tests;

public class CompatibilityTests
{
    public static readonly string[] TreeTypes = { "reference", "light" };

    public IntervalTree.IIntervalTree<long, int> CreateEmptyTree(string type) => type switch
    {
        "reference" => new IntervalTree.IntervalTree<long, int>(),
        "light" => new TreeAdapter<long, int>(new LightIntervalTree.LightIntervalTree<long, int>()),
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

    public class EnumeratingTreeValues : CompatibilityTests
    {
        [Test]
        [TestCaseSource(nameof(TreeTypes))]
        public void OfAEmptyTree_ShouldYield0Values(string treeType)
        {
            var tree = CreateEmptyTree(treeType);

            var values = tree.Values;

            Assert.That(values.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(TreeTypes))]
        public void OfATreeWith1Interval_ShouldYield1Value(string treeType)
        {
            var tree = CreateEmptyTree(treeType);
            tree.Add(1, 2, 1);

            var values = tree.Values;

            Assert.That(values.Count, Is.EqualTo(1));
            Assert.That(values, Is.EquivalentTo(Enumerable.Range(1, 1)));
        }

        [Test]
        [TestCaseSource(nameof(TreeTypes))]
        public void OfATreeWith10Intervals_ShouldYield10Values(string treeType)
        {
            var tree = CreateEmptyTree(treeType);
            for (int i = 0; i < 10; i++)
            {
                tree.Add(i, i + 1, i);
            }

            var values = tree.Values;

            Assert.That(values.Count, Is.EqualTo(10));
            Assert.That(values, Is.EquivalentTo(Enumerable.Range(0, 10)));
        }

        [Test]
        [TestCaseSource(nameof(TreeTypes))]
        public void OfATreeWithDuplicateValues_ShouldYieldDuplicateValues(string treeType)
        {
            var tree = CreateEmptyTree(treeType);
            tree.Add(1, 2, 1);
            tree.Add(2, 3, 1);
            tree.Add(3, 4, 1);

            var values = tree.Values;

            Assert.That(values.Count, Is.EqualTo(3));
            Assert.That(values, Is.EquivalentTo(new List<int>() { 1, 1, 1 }));
        }

        [Test]
        [TestCaseSource(nameof(TreeTypes))]
        public void OfATreeWithDuplicateIntervals_ShouldYieldDuplicateValues(string treeType)
        {
            var tree = CreateEmptyTree(treeType);
            tree.Add(1, 2, 1);
            tree.Add(1, 2, 1);
            tree.Add(1, 2, 1);

            var values = tree.Values;

            Assert.That(values.Count, Is.EqualTo(3));
            Assert.That(values, Is.EquivalentTo(new List<int>() { 1, 1, 1 }));
        }
    }

    public class OutputTests : CompatibilityTests
    {
        [Test]
        public void CompareResults(
            [Values("light")] string treeType,
            [Range(1, 25)] int seed)
        {
            var random = new Random(seed);

            var intervals = Enumerable.Range(0, 16)
                .Select(_ => random.NextInt64(0, 100))
                .Select((from, i) => new Interval { From = from, To = from + random.Next(1, 10), Value = i })
                .ToList();

            var originalTree = CreateEmptyTree("reference");
            var treeUnderTest = CreateEmptyTree(treeType);

            foreach (var interval in intervals)
            {
                originalTree.Add(interval.From, interval.To, interval.Value);
                treeUnderTest.Add(interval.From, interval.To, interval.Value);
            }

            for (var i = 0; i < 10_100; i++)
            {
                var reference = originalTree.Query(i);
                var result = treeUnderTest.Query(i);

                Assert.That(result, Is.EquivalentTo(reference), $"Result mismatch for Query: {i}, TreeType: {treeType}, Seed: {seed}");
            }
        }
    }
}
