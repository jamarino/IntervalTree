using NUnit.Framework;
using System;
using System.Linq;
using Extras;

namespace Tests;

public class CompatibilityTests
{ 
    [Test]
    public void CompareQuery(
        [ValueSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypesSansReference))]
        string treeType,
        [Range(1, 500)]
        int seed)
    {
        const int IntervalMaxCount = 20;
        const int IntervalMax = 100;
        var random = new Random(seed);

        var intervals = Enumerable.Range(0, random.Next(2, IntervalMaxCount))
            .Select(_ => random.NextInt64(0, IntervalMax))
            .Select((from, i) => new Jamarino.IntervalTree.Interval<long, int>(from, from + random.Next(1, IntervalMax), i))
            .ToList();

        var originalTree = TreeFactory.CreateEmptyTree<long, int>("reference");
        var treeUnderTest = TreeFactory.CreateEmptyTree<long, int>(treeType);

        foreach (var interval in intervals)
        {
            originalTree.Add(interval.From, interval.To, interval.Value);
            treeUnderTest.Add(interval.From, interval.To, interval.Value);
        }

        for (var i = 0; i < IntervalMax * 2; i++)
        {
            var reference = originalTree.Query(i);
            var result = treeUnderTest.Query(i);

            Assert.That(result, Is.EquivalentTo(reference), $"Result mismatch for Query: {i}, TreeType: {treeType}, Seed: {seed}");
        }
    }

    [Test]
    public void CompareQueryRange(
        [ValueSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypesSansReference))]
        string treeType,
        [Range(1, 500)]
        int seed)
    {
        const int IntervalMaxCount = 20;
        const int IntervalMax = 100;
        var random = new Random(seed);

        var intervals = Enumerable.Range(0, random.Next(2, IntervalMaxCount))
            .Select(_ => random.NextInt64(0, IntervalMax))
            .Select((from, i) => new Jamarino.IntervalTree.Interval<long, int>(from, from + random.Next(1, IntervalMax), i))
            .ToList();

        var originalTree = TreeFactory.CreateEmptyTree<long, int>("reference");
        var treeUnderTest = TreeFactory.CreateEmptyTree<long, int>(treeType);

        foreach (var interval in intervals)
        {
            originalTree.Add(interval.From, interval.To, interval.Value);
            treeUnderTest.Add(interval.From, interval.To, interval.Value);
        }

        for (var i = 0; i < IntervalMax * 2; i++)
        {
            var range = random.Next(1, IntervalMax);
            var reference = originalTree.Query(i, i + range);
            var result = treeUnderTest.Query(i, i + range);

            Assert.That(result, Is.EquivalentTo(reference), $"Result mismatch for Query: {i}, TreeType: {treeType}, Seed: {seed}");
        }
    }

    [Test]
    public void CompareQueryIncrementalSize(
        [ValueSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypesSansReference))]
        string treeType)
    {
        const int MaxSize = 260;
        var random = new Random(123);

        var originalTree = TreeFactory.CreateEmptyTree<long, int>("reference");
        var treeUnderTest = TreeFactory.CreateEmptyTree<long, int>(treeType);

        for (var i = -MaxSize; i < MaxSize; i++)
        {
            originalTree.Add(i, i+1, i);
            treeUnderTest.Add(i, i+1, i);

            for (int j = -MaxSize; j <= i; j++)
            {
                var reference = originalTree.Query(i);
                var result = treeUnderTest.Query(i);
                Assert.That(result, Is.EquivalentTo(reference), $"Result mismatch for Query: {j}, TreeType: {treeType}, Iteration: {i}");
            }
        }
    }
}
