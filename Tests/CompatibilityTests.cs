using NUnit.Framework;
using System;
using System.Linq;
using Extras;

namespace Tests;

public class CompatibilityTests
{ 
    [Test]
    public void CompareResults(
        [ValueSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypesSansReference))]
        string treeType,
        [Range(1, 100)]
        int seed)
    {
        const int IntervalMaxCount = 100;
        const int IntervalMax = 100;
        var random = new Random(seed);

        var intervals = Enumerable.Range(0, random.Next(3, IntervalMaxCount))
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
}
