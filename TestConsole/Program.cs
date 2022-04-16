const long RangeMin = 300_000_000_000;
const long RangeMax = 700_000_000_000;
var random = new Random(123123);
var ranges = Enumerable.Range(0, 300_000)
    .Select(i => random.NextInt64(RangeMin / 1000, RangeMax / 1000))
    .Select(i => (i * 1000, (i * 1000) + (random.Next(1, 3) * 1000) - 1, random.Next(10_000)))
    .ToList();

var tests = Enumerable.Range(0, 10)
    .Select(_ => random.NextInt64(RangeMin, RangeMax))
    .ToList();

//var tree = new IntervalTree.IntervalTree<long, int>();
var tree = new LightIntervalTree.LargeSparseIntervalTree<long, int>();

foreach (var range in ranges)
{
    tree.Add(range.Item1, range.Item2, range.Item3);
}

//tree.Query(0);
tree.OptimizeRecursive();

foreach (var test in tests)
{
    tree.Query(test);
}

Console.WriteLine("Press enter to terminate");
Console.ReadLine();