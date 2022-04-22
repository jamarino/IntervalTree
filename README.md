# LightIntervalTree

A light-weight interval tree in C#. Heavily inspired by [RangeTree (GitHub)](https://github.com/mbuchetics/RangeTree), but this project provides a completely new implementation that is, from scratch, focused on reducing memory usage and allocations. `RangeTree` is still a great option if you need a fully featured interval tree.

This implementation uses a combination of of concepts from [Centered Interval Trees (Wikipedia)](https://en.wikipedia.org/wiki/Interval_tree#Centered_interval_tree) - for querying - and [AVL trees (Wikipedia)](https://en.wikipedia.org/wiki/AVL_tree) - for self-balancing.

## Limitations

1. The feature set is currently quite limited, `LightIntervalTree` only supports adding intervals and querying for specific values.

1. `LightIntervalTree`s are balanced (if needed) on every `Add()` call. This is necesarry keep the internal AVL tree balanced and avoids keeping duplicate data. It has the benefit of offering consistent query times, where `RangeTree` will rebuild the whole tree of the first query after and addition or deletion.

1. `LightIntervalTree`s are limited to approximately 2 billion intervals. This is because `int`s are used as "pointers" as an optimization.

1. `LightIntervalTree`s is optimised for "sparse" data sets, i.e. intervals with few or no overlaps. For dense data sets (avg. 5+ overlaps), try using [RangeTree (GitHub)](https://github.com/mbuchetics/RangeTree) instead.

## Example

```csharp
// create a tree
var tree = new LightIntervalTree<short, short>();

// add intervals
tree.Add(100, 200, 1);
tree.Add(120, 150, 2);
tree.Add(110, 250, 3);

// query for a value
var results = tree.Query(123);

// result is {1, 2, 3} (no order guarantee)
```

## Performance

Testing shows that for sparse data sets, `LightIntervalTree` uses just 1/10th of the memory compared to `RangeTree` while offering similar or better construction and query times.

Denser data sets with an average of ~5 overlaps use 1/3rd the memory compared to `RangeTree` while still offering comparable construction and query times.

### Load 300 000 sparse intervals

| Method |  TreeType |     Mean |    Error |   StdDev |      Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|------- |---------- |---------:|---------:|---------:|-----------:|----------:|----------:|----------:|
|   Load |     light | 136.1 ms |  2.57 ms |  2.40 ms |   250.0000 |  250.0000 |  250.0000 |     48 MB |
|   Load | reference | 538.0 ms | 10.32 ms | 11.47 ms | 28000.0000 | 7000.0000 | 2000.0000 |    623 MB |

> Note: Allocated memory is different from memory usage. It describes to total amount of memory written, not how much was ultimately kept.

###  Query trees of 100 000 intervals

| Method |  TreeType |   DataType |     Mean |    Error |   StdDev |
|------- |---------- |----------- |---------:|---------:|---------:|
|  Query |     light |      dense | 825.7 ns | 16.37 ns | 20.11 ns |
|  Query |     light |     sparse | 239.7 ns |  4.61 ns |  4.93 ns |
|  Query | reference |      dense | 918.4 ns | 17.37 ns | 17.06 ns |
|  Query | reference |     sparse | 460.4 ns |  8.60 ns |  8.04 ns |

### Memory usage

In order to gauge real-world memory performance, this repository includes a `TestConsole` project which allows instantiating a tree with several parameters and print out the current memory usage. It is used by running:

```sh
> ./TestConsole.exe memtest light --count 1000000 --interval-step 1 --interval-max 10000 --interval-max-size 2000
TotalComittedBytes: 49720 KB
> ./TestConsole.exe memtest reference --count 1000000 --interval-step 1 --interval-max 10000 --interval-max-size 2000
TotalComittedBytes: 235776 KB
```

In short, the console generates `--count` number of random intervals and adds them to either a `reference` tree (RangeTree) or a `light` tree (LightIntervalTree). The other parameters control the maximum starting coordinate of the intervals `--interval-max` (random 0 to max), the maximum size of the intervals `--interval-max-size` (random 1 to max), and the alignment of the intervals in steps `--interval-step` (i.e. step X means all starting coordinates are integer divisible by X).

The following was run with a `--interval-step 1` and an `--interval-max 1000000`.

Summary;
| Interval Count | Interval Max Size | Memory (Reference) | Memory (Light) |     Ratio |
|---------------:|------------------:|-------------------:|---------------:|----------:|
|        100 000 |               100 |            35.7 MB |     **8.2 MB** |  **0.23** |
|      1 000 000 |               100 |           190.3 MB |    **29.7 MB** |  **0.16** |
|     10 000 000 |               100 |          2595.7 MB |   **203.6 MB** |  **0.08** |
|        100 000 |              1000 |            30.2 MB |     **4.0 MB** |  **0.13** |
|      1 000 000 |              1000 |           148.7 MB |    **50.0 MB** |  **0.34** |
|     10 000 000 |              1000 |          3959.3 MB |   **197.5 MB** |  **0.05** |

## Thread Safety

Concurrent reads are safe, but adding intervals requires exclusive access. It is up to the consumer to enforce synchronization controls. Consider using something like [ReaderWriterLockSlim (Microsoft)](https://docs.microsoft.com/en-us/dotnet/api/system.threading.readerwriterlockslim).

## TODO list

* Order intervals within nodes to optimize queries
* Add property for enumerating intervals
* Add method for querying a range
* Create a light-er class using shorts for internal indecies
* Consider storing reverse-ordered intervals within nodes to further optimize queries
* Consider supporting remove methods

## Optimizations over RangeTree

A few key design decisions were made to reduce the memory usage.

1. Avoid keeping duplicate data
    * `RangeTree` keeps a full copy of intervals, in case the tree needs to be rebuilt following the addition or removal of an interval. `LightIntervalTree` only stores intervals as part of the underlying tree structure.
1. Model tree nodes as value types (`struct`) rather than objects (`class`)
    * Objects suffer memory overhead in the form of type and method information
    * Since `struct`s cannot reference themselves and index (`int`) is used to reference other nodes
1. Store nodes and intervals in indexable lists, use indexes rather than references as pointers
    * Pointers in 64-bit systems take up 8 bytes of storage, `int`s only take 4 bytes
    * Storing value types in Lists/Arrays may improve CPU caching since elements are co-located
1. Nodes store their intervals in linked lists
    * Nodes use indexes to point to the first interval in their list. Each interval stores an additional index pointing to the next interval (if present) to form a "linked list".
    * For sparse trees this means that the majority of nodes will be storing two ints (one in the node and one in the single interval for that node) as opposed to allocating a 1-length array and storing an 8 byte pointer to said array.
