using NUnit.Framework;
using Extras;

namespace Tests;

public class RegressionTests
{
    public class LobsidedTreesCauseUndersizedStackSizeForDfs
    {
        [Test]
        public void QueryStackIndexOutOfRange(
            [ValueSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypesSansReference))]
            string treeType)
        {
            var treeUnderTest = TreeFactory.CreateEmptyTree<long, int>(treeType);

            var from = 0;
            var groupSize = 256;
            while (groupSize > 0)
            {
                for (var i = 0; i < groupSize; i++)
                {
                    treeUnderTest.Add(from, from + 1, i);
                }

                groupSize /= 2;
                from += 2;
            }

            Assert.DoesNotThrow(() => treeUnderTest.Query(from + 1));
        }

        [Test]
        public void QueryStackIndexOutOfRangeMirrored(
        [ValueSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypesSansReference))]
            string treeType)
        {
            var treeUnderTest = TreeFactory.CreateEmptyTree<long, int>(treeType);

            var from = 0;
            var groupSize = 1;
            while (groupSize <= 256)
            {
                for (var i = 0; i < groupSize; i++)
                {
                    treeUnderTest.Add(from, from + 1, i);
                }

                groupSize *= 2;
                from += 2;
            }

            Assert.DoesNotThrow(() => treeUnderTest.Query(-1));
        }

        [Test]
        public void QueryRangeStackIndexOutOfRange(
            [ValueSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypesSansReference))]
            string treeType)
        {
            var treeUnderTest = TreeFactory.CreateEmptyTree<long, int>(treeType);

            var from = 0;
            var groupSize = 256;
            while (groupSize > 0)
            {
                for (var i = 0; i < groupSize; i++)
                {
                    treeUnderTest.Add(from, from + 1, i);
                }

                groupSize /= 2;
                from += 2;
            }

            Assert.DoesNotThrow(() => treeUnderTest.Query(0, from + 1));
        }

        [Test]
        public void QueryRangeStackIndexOutOfRangeMirrored(
            [ValueSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypesSansReference))]
            string treeType)
        {
            var treeUnderTest = TreeFactory.CreateEmptyTree<long, int>(treeType);

            var from = 0;
            var groupSize = 1;
            while (groupSize <= 256)
            {
                for (var i = 0; i < groupSize; i++)
                {
                    treeUnderTest.Add(from, from + 1, i);
                }

                groupSize *= 2;
                from += 2;
            }

            Assert.DoesNotThrow(() => treeUnderTest.Query(0, from + 1));
        }
    }
}
