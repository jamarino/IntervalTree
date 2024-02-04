using Cocona;
using Extras;
using System.Diagnostics;

var builder = CoconaLiteApp.CreateBuilder();

var app = builder.Build();

app.AddCommand("memtest", (
    [Argument]
    string treeType,
    int? treeCapacity,
    int? seed,
    int? count,
    long? intervalMax,
    int? intervalStep,
    long? intervalMaxSize,
    int? memoryMax,
    bool? verbose,
    bool? hang,
    int? treesMax,
    CoconaAppContext context) =>
{
    seed ??= 0;
    count ??= 100_000;
    intervalMax ??= 1_000_000;
    intervalStep ??= 1;
    intervalMaxSize ??= 100;
    memoryMax ??= 1_000_000_000;
    verbose ??= true;
    hang ??= false;
    treesMax ??= 25;

    var random = new Random(seed.Value);
    using var p = Process.GetCurrentProcess();
    p.Refresh();
    var initialMemoryUsage = p.PrivateMemorySize64;
    var previousMemoryUsage = initialMemoryUsage;
    if (verbose.Value)
        Console.WriteLine($"Initial memory usage: {initialMemoryUsage / 1024} KB");
    var trees = new List<IntervalTree.IIntervalTree<long, int>>();

    while (trees.Count < treesMax.Value)
    {
        var tree = treeCapacity is not null ?
            TreeFactory.CreateEmptyTree<long, int>(treeType, treeCapacity) :
            TreeFactory.CreateEmptyTree<long, int>(treeType);
        
        trees.Add(tree);

        for (int i = 0; i < count.Value; i++)
        {
            var from = random.NextInt64(0, intervalMax.Value) / intervalStep.Value * intervalStep.Value;
            var to = from + random.NextInt64(0, Math.Min(intervalMaxSize.Value, intervalMax.Value - from + 1)) + 1;
            tree.Add(from, to, i);
        }

        tree.Query(0);

        p.Refresh();
        var privateBytes = p.PrivateMemorySize64;
        var memoryDelta = privateBytes - previousMemoryUsage;
        previousMemoryUsage = privateBytes;

        if (verbose.Value)
            Console.WriteLine($"Tree count: {trees.Count,3}. " +
                $"Total private memory: {privateBytes / 1024 / 1024,5} MB. " +
                $"Memory delta: {memoryDelta / 1024 / 1024,5} MB. " +
                $"Avg memory per tree: {(privateBytes-initialMemoryUsage) / 1024 / 1024 / trees.Count,5} MB");

        if ((double)(trees.Count + 1) / trees.Count * privateBytes > memoryMax)
            break;
    }

    Console.WriteLine($"Tree count: {trees.Count}");
    Console.WriteLine($"Total private memory: {p.PrivateMemorySize64 / 1024 / 1024} MB");
    Console.WriteLine($"Memory per tree: {(p.PrivateMemorySize64-initialMemoryUsage) / 1024 / 1024 / trees.Count} MB");

    if (hang.Value)
    {
        Console.WriteLine("Press Enter to terminate.");
        Console.ReadLine();
    }
});

app.AddCommand("maxsize", (
    [Argument]
    string treeType
    ) =>
{
    var size = 1000;
    var prevSizeChange = 1000;

    while(true)
    {
        try
        {
            // init tree
            Console.WriteLine($"Trying size: {size}");
            var tree = TreeFactory.CreateEmptyTree<long, long>(treeType);

            // load tree
            for (int i = 0; i < size; i++)
            {
                tree.Add(i, i + 1, i);
            }

            // build
            tree.Query(1);

            // no boom?
            var tmp = size;
            size = size + prevSizeChange;
            prevSizeChange = tmp;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            prevSizeChange /= 2;
            size = size - prevSizeChange;
        }
    }
});

app.AddCommand("concurrentbuild", (
    [Argument]
    string treeType
    ) =>
{
    const int numTrees = 10;
    const int numIntervals = 100_000;
    const int intervalMax = 1000;
    const int numThreads = 10;

    var trees = Enumerable
        .Range(0, numTrees)
        .Select((i) => TreeFactory.CreateEmptyTree<long, int>(treeType))
        .ToArray();

    var rand = new Random(123);

    for (var i = 0; i < numIntervals; i++)
    {
        var from = rand.Next(intervalMax);
        var to = from + rand.Next(1, 100);

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
                    {
                        Console.WriteLine($"Thread {tId}: Query {i} tree {j}. Result has different length: Control {control.Length}, Test: {test.Length}");
                        continue;
                    }

                    for (int k = 0; k < control.Length; k++)
                    {
                        if (test[k] != control[k])
                        {
                            Console.WriteLine($"Thread {tId}: Query {i} tree {j}. Result has different content: Control {control[k]}, Test: {test[k]}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Thread {tId}: Query {i} tree {j}. Exception thrown:\n{e.StackTrace}");
                }
            }
        }
    });
});

app.AddCommand("intervalgen", (
    [Argument]
    int count,
    int? seed
    ) =>
{
    var rand = new Random(seed ?? 123);
    var data = IntervalGenerator.GenerateLongInt(count, rand);

    Console.WriteLine("Value\tFrom\tTo");
    foreach (var item in data)
    {
        Console.WriteLine($"{item.Value}\t{item.From}\t{item.To}");
    }
});

await app.RunAsync();
