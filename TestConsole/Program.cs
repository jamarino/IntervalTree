using Cocona;
using Extras;

var builder = CoconaLiteApp.CreateBuilder();

var app = builder.Build();

app.AddCommand("memtest", (
    [Argument]
    string treeType,
    int? seed,
    int? count,
    long? intervalMax,
    int? intervalStep,
    long? intervalMaxSize
    ) =>
{
    seed ??= 0;
    count ??= 100_000;
    intervalMax ??= 1_000_000;
    intervalStep ??= 1_000;
    intervalMaxSize ??= 5_000;

    IntervalTree.IIntervalTree<long, int> tree = treeType switch
    {
        "original" => new IntervalTree.IntervalTree<long, int>(),
        "largesparse" => new TreeAdapter<long, int>(new LightIntervalTree.LargeSparseIntervalTree<long, int>()),
        _ => throw new Exception($"Unknown tree type: {treeType}")
    };

    var random = new Random(seed.Value);

    for (int i = 0; i < count.Value; i++)
    {
        var from = random.NextInt64(0, intervalMax.Value) / intervalStep.Value * intervalStep.Value;
        var to = from + random.NextInt64(1, intervalMaxSize.Value);
        var val = random.Next(10_000);
        tree.Add(from, to, val);
    }

    tree.Query(0);

    var memInfo = GC.GetGCMemoryInfo();
    Console.WriteLine($"TotalComittedBytes: {memInfo.TotalCommittedBytes / 1024} KB");
});

await app.RunAsync();