using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Extras;

namespace Tests;

public class UnitTests
{
    public class AnEmptyTree : UnitTests
    {
        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueried_Returns0Results(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);

            var results = tree.Query(0);

            Assert.That(results.Count(), Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedRange_Returns0Results(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);

            var results = tree.Query(0, 10);

            Assert.That(results.Count(), Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenAddedTo_DoesNotExplode(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);

            Assert.DoesNotThrow(() => tree.Add(10, 20, 1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenAddedTo_IncreasesCountTo1(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);

            tree.Add(10, 20, 1);

            Assert.That(tree.Count, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenCleared_RemainsEmpty(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);

            tree.Clear();

            Assert.That(tree.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenRemoving_DoesNothing(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);

            tree.Remove(123);

            Assert.That(tree.Count, Is.EqualTo(0));
        }
    }

    public class ATreeWithOneInterval : UnitTests
    {
        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedInInterval_ReturnsMatch(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);

            var results = tree.Query(15);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First(), Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedAtFromValue_ReturnsMatch(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);

            var results = tree.Query(10);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First(), Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedAtToValue_ReturnsMatch(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);

            var results = tree.Query(19);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First(), Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedBeforeFromValue_Returns0Matches(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);

            var results = tree.Query(9);

            Assert.That(results.Count(), Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedAfterToValue_Returns0Matches(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);

            var results = tree.Query(20);

            Assert.That(results.Count(), Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedRangeBeforeFromValue_Returns0Matches(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);

            var results = tree.Query(5, 8);

            Assert.That(results.Count(), Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedRangeAcrossFromValue_ReturnsMatch(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);

            var results = tree.Query(8, 12);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First(), Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedRangeInsideInterval_ReturnsMatch(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);

            var results = tree.Query(12, 15);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First(), Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedRangeAcrossTo_ReturnsMatch(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);

            var results = tree.Query(18, 23);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First(), Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedRangeAfterToValue_Returns0Matches(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);

            var results = tree.Query(22, 25);

            Assert.That(results.Count(), Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenCleared_Returns0Count(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 20, 1);

            tree.Clear();

            Assert.That(tree.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenCleared_Returns0Results(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 20, 1);

            tree.Clear();
            var results = tree.Query(15);

            Assert.That(results.Count(), Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenRemovingValue_IsCount0(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 20, 1);

            tree.Remove(1);

            Assert.That(tree.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenRemovingValue_IsEmpty(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 20, 1);

            tree.Remove(1);

            Assert.That(tree.Count(), Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenRemovingOtherValue_IsCount0(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 20, 1);

            tree.Remove(1337);

            Assert.That(tree.Count, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenRemovingOtherValue_IsEmpty(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 20, 1);

            tree.Remove(1337);

            Assert.That(tree.Count(), Is.EqualTo(1));
        }
    }

    public class ATreeWithTwoDisjointIntervals : UnitTests
    {
        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void HasACountOfTwo(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);
            tree.Add(50, 59, 2);

            Assert.That(tree.Count, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void ThrowsWhenAddingReverseInterval(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            
            tree.Add(10, 10, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                tree.Add(20, 10, 1);
            });          
        }
        
        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedInFirstInterval_ReturnsValueOfFirstInterval(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);
            tree.Add(50, 59, 2);

            var results = tree.Query(15);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.First(), Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedInSecondInterval_ReturnsValueOfSecondInterval(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);
            tree.Add(50, 59, 2);

            var results = tree.Query(55);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.First(), Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedBeforeFirstInterval_Returns0Results(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);
            tree.Add(50, 59, 2);

            var results = tree.Query(5);

            Assert.That(results.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedBetweenIntervals_Returns0Results(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);
            tree.Add(50, 59, 2);

            var results = tree.Query(35);

            Assert.That(results.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedAfterSecondInterval_Returns0Results(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);
            tree.Add(50, 59, 2);

            var results = tree.Query(75);

            Assert.That(results.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenValueRemoved_CountIs1(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);
            tree.Add(50, 59, 2);

            tree.Remove(1);

            Assert.That(tree.Count, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenFirstValueRemoved_SecondValueRemains(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);
            tree.Add(50, 59, 2);

            tree.Remove(1);

            Assert.That(tree.Any(i => i.Value == 2), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenSecondValueRemoved_FirstValueRemains(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);
            tree.Add(50, 59, 2);

            tree.Remove(2);

            Assert.That(tree.Any(i => i.Value == 1), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenNoValuesAreRemoved_BothValuesRemain(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 1);
            tree.Add(50, 59, 2);

            tree.Remove(3);

            Assert.That(tree.Count, Is.EqualTo(2));
            Assert.That(tree.Any(i => i.Value == 1), Is.True);
            Assert.That(tree.Any(i => i.Value == 2), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenBothValuesAreRemoved_NoValuesRemain(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 19, 2);
            tree.Add(50, 59, 2);

            tree.Remove(2);

            Assert.That(tree.Count, Is.EqualTo(0));
            Assert.That(tree.Any(i => i.Value == 2), Is.False);
        }
    }

    public class ATreeWithTwoPatiallyOverlappingIntervals : UnitTests
    {
        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void HasACountOfTwo(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 29, 1);
            tree.Add(20, 39, 2);

            Assert.That(tree.Count, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedInFirstInterval_ReturnsValueOfFirstInterval(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 29, 1);
            tree.Add(20, 39, 2);

            var results = tree.Query(15);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.First(), Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedInSecondInterval_ReturnsValueOfSecondInterval(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 29, 1);
            tree.Add(20, 39, 2);

            var results = tree.Query(35);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.First(), Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedBeforeFirstInterval_Returns0Results(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 29, 1);
            tree.Add(20, 39, 2);

            var results = tree.Query(5);

            Assert.That(results.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedInOverlap_ReturnsBothIntervalValues(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 29, 1);
            tree.Add(20, 39, 2);

            var results = tree.Query(25);

            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results.Contains(1), Is.True);
            Assert.That(results.Contains(2), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedAfterSecondInterval_Returns0Results(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 29, 1);
            tree.Add(20, 39, 2);

            var results = tree.Query(75);

            Assert.That(results.Count, Is.EqualTo(0));
        }
    }

    public class ATreeWithTwoCompletelyOverlappingIntervals : UnitTests
    {
        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void HasACountOfTwo(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 49, 1);
            tree.Add(20, 39, 2);

            Assert.That(tree.Count, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedInFirstIntervalBeforeSecondInterval_ReturnsValueOfFirstInterval(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 49, 1);
            tree.Add(20, 39, 2);

            var results = tree.Query(15);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.First(), Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedInFirstIntervalAfterSecondInterval_ReturnsValueOfFirstInterval(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 49, 1);
            tree.Add(20, 39, 2);

            var results = tree.Query(45);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.First(), Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedBeforeFirstInterval_Returns0Results(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 49, 1);
            tree.Add(20, 39, 2);

            var results = tree.Query(5);

            Assert.That(results.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedInOverlap_ReturnsBothIntervalValues(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 49, 1);
            tree.Add(20, 39, 2);

            var results = tree.Query(25);

            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results.Contains(1), Is.True);
            Assert.That(results.Contains(2), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void WhenQueriedAfterFirstInterval_Returns0Results(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(10, 49, 1);
            tree.Add(20, 39, 2);

            var results = tree.Query(75);

            Assert.That(results.Count, Is.EqualTo(0));
        }
    }

    public class EnumeratingTreeValues : UnitTests
    {
        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void OfAEmptyTree_ShouldYield0Values(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);

            var values = tree.Values;

            Assert.That(values.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void OfATreeWith1Interval_ShouldYield1Value(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(1, 2, 1);

            var values = tree.Values;

            Assert.That(values.Count, Is.EqualTo(1));
            Assert.That(values, Is.EquivalentTo(Enumerable.Range(1, 1)));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void OfATreeWith10Intervals_ShouldYield10Values(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            for (int i = 0; i < 10; i++)
            {
                tree.Add(i, i + 1, i);
            }

            var values = tree.Values;

            Assert.That(values.Count, Is.EqualTo(10));
            Assert.That(values, Is.EquivalentTo(Enumerable.Range(0, 10)));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void OfATreeWithDuplicateValues_ShouldYieldDuplicateValues(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(1, 2, 1);
            tree.Add(2, 3, 1);
            tree.Add(3, 4, 1);

            var values = tree.Values;

            Assert.That(values.Count, Is.EqualTo(3));
            Assert.That(values, Is.EquivalentTo(new List<int>() { 1, 1, 1 }));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void OfATreeWithDuplicateIntervals_ShouldYieldDuplicateValues(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(1, 2, 1);
            tree.Add(1, 2, 1);
            tree.Add(1, 2, 1);

            var values = tree.Values;

            Assert.That(values.Count, Is.EqualTo(3));
            Assert.That(values, Is.EquivalentTo(new List<int>() { 1, 1, 1 }));
        }
    }

    public class Enumerating : UnitTests
    {
        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void AnEmptyTree_ShouldYield0Intervals(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);

            var intervals = tree.ToList();

            Assert.That(intervals.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void ATreeWith1Interval_ShouldYield1Interval(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(1, 2, 1);

            var intervals = tree.ToList();

            Assert.That(intervals.Count, Is.EqualTo(1));
            Assert.That(intervals, Is.EquivalentTo(new List<IntervalTree.RangeValuePair<long, int>> { new IntervalTree.RangeValuePair<long, int>(1, 2, 1) }));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void ATreeWith10Intervals_ShouldYield10Intervals(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            var expectedIntervals = 
                Enumerable.Range(0, 10)
                    .Select(i => new IntervalTree.RangeValuePair<long, int>(i, i + 1, i))
                    .ToList();

            foreach(var interval in expectedIntervals)
            {
                tree.Add(interval.From, interval.To, interval.Value);
            }

            var intervals = tree.ToList();

            Assert.That(intervals.Count, Is.EqualTo(10));
            Assert.That(intervals, Is.EquivalentTo(expectedIntervals));
        }

        [Test]
        [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
        public void ATreeWithDuplicateIntervals_ShouldYieldDuplicateValues(string treeType)
        {
            var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
            tree.Add(1, 2, 1);
            tree.Add(1, 2, 1);
            tree.Add(1, 2, 1);

            var intervals = tree.ToList();

            Assert.That(intervals.Count, Is.EqualTo(3));
            Assert.That(intervals, Is.EquivalentTo(Enumerable.Range(0, 3).Select(_ => new IntervalTree.RangeValuePair<long, int>(1, 2, 1))));
        }
    }
}
