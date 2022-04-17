const long RangeMin = 300_000_000_000;
const long RangeMax = 700_000_000_000;
const int RangeCount = 2_000_000;

var random = new Random(123123);
//var ranges = Enumerable.Range(0, RangeCount)
//    .Select(i => random.NextInt64(RangeMin / 1000, RangeMax / 1000))
//    .Select(i => (i * 1000, (i * 1000) + (random.Next(1, 3) * 1000) - 1, random.Next(10_000)))
//    .ToList();

//var tests = Enumerable.Range(0, 10)
//    .Select(_ => random.NextInt64(RangeMin, RangeMax))
//    .ToList();

//var tree = new IntervalTree.IntervalTree<long, int>();
var tree = new LightIntervalTree.LargeSparseIntervalTree<long, int>();
//var tree = new LightIntervalTree.LargeDenseIntervalTree<long, int>();

//foreach (var range in ranges)
//{
//    tree.Add(range.Item1, range.Item2, range.Item3);
//}

for (int i = 0; i < RangeCount; i++)
{
    var from = random.NextInt64(RangeMin, RangeMax) / 1000 * 1000;
    var to = from + random.Next(1, 3) * 1000;
    var val = random.Next(10_000);
    tree.Add(from, to, val);
}

tree.Query(0);
//tree.Optimize();

//foreach (var test in tests)
//{
//    tree.Query(test);
//}

await Task.Delay(TimeSpan.FromSeconds(1));

var memInfo = GC.GetGCMemoryInfo();

//Console.WriteLine($"MemoryLoadBytes: {memInfo.MemoryLoadBytes/1024} KB");
//Console.WriteLine($"HeapSizeBytes: {memInfo.HeapSizeBytes/1024} KB");
Console.WriteLine($"TotalComittedBytes: {memInfo.TotalCommittedBytes/1024} KB");
//Console.WriteLine($"GetTotalMemory: {GC.GetTotalMemory(false)/1024} KB");

//Console.WriteLine("Press enter to terminate");
//Console.ReadLine();