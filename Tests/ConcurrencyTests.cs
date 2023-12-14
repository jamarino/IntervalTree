using NUnit.Framework;
using System;
using System.Linq;
using Extras;
using System.Threading.Tasks;

namespace Tests;

public class ConcurrencyTests
{
    [Test]
    [TestCaseSource(typeof(TreeFactory), nameof(TreeFactory.TreeTypes))]
    public void ConcurrentQueriesBuildSafely(string treeType)
    {
        const int numTrees = 5;
        const int numIntervals = 50_000;
        const int intervalMax = 1200;
        const int numThreads = 10;

        var trees = Enumerable
            .Range(0, numTrees)
            .Select((i) => TreeFactory.CreateEmptyTree<long, int>(treeType))
            .ToArray();

        var rand = new Random(123);

        for (var i = 0; i < numIntervals; i++)
        {
            var from = rand.Next((int)(0.8 * intervalMax));
            var to = from + rand.Next(1, (int)(0.2 * intervalMax));

            foreach (var t in trees)
            {
                t.Add(from, to, i);
            }
        }

        // init tree #0 as control
        trees[0].Query(1);

        var result = Parallel.For(0, numThreads, (tId) =>
        {
            for (var i = 0; i < intervalMax; i++)
            {
                var control = trees[0].Query(i).ToArray();

                for (int j = 1; j < numTrees; j++)
                {
                    try
                    {
                        var test = trees[j].Query(i).ToArray();

                        if (test.Length != control.Length)
                            Assert.Fail($"Thread {tId}: Query {i} tree {j}. Result has different length: Control {control.Length}, Test: {test.Length}");
                        

                        for (int k = 0; k < control.Length; k++)
                            if (test[k] != control[k])
                                Assert.Fail($"Thread {tId}: Query {i} tree {j}. Result has different content: Control {control[k]}, Test: {test[k]}");
                    }
                    catch (Exception e)
                    {
                        Assert.Fail($"Thread {tId}: Query {i} tree {j}. Exception thrown:\n{e.StackTrace}");
                    }
                }
            }
        });

        if (!result.IsCompleted)
            Assert.Fail("Not all parallel iterations completed.");
    }
}
