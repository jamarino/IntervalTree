using Cocona;
using Extras;
using System.Diagnostics;

var builder = CoconaLiteApp.CreateBuilder();

var app = builder.Build();

app.AddCommand("memtest", (
    [Argument]
    string treeType,
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
        var tree = TreeFactory.CreateEmptyTree<long, int>(treeType);
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

await app.RunAsync();
