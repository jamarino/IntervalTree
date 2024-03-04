# Jamarino.IntervalTree

Light-weight, performant interval trees written in C#.

Designed as a drop-in replacement for the popular [RangeTree (GitHub)](https://github.com/mbuchetics/RangeTree) package. This project provides a completely new implementation that is, from scratch, focused on reducing memory usage and allocations.

About 3 times the query performance, and as little as 10% of the peak memory usage! See [BENCHMARKS.md](BENCHMARKS.md) for more details.

## Example

```csharp
// create a tree
var tree = new LightIntervalTree<int, short>();

// add intervals (from, to, value)
tree.Add(10, 30, 1);
tree.Add(20, 40, 2);
tree.Add(25, 35, 3);

// query
tree.Query(11); // result is {1}
tree.Query(32); // result is {2, 3}
tree.Query(27); // result is {1, 2, 3}

// query range
tree.Query(5, 20) // result is {1, 2}
tree.Query(26, 28) // result is {1, 2, 3}
tree.Query(1, 50) // result is {1, 2, 3}

// note: result order is not guaranteed
```

## Trees

This package currently offers three different interval tree implementations:

1. `LightIntervalTree`
    * Simple, fast, light on memory
1. `QuickIntervalTree`
    * Speed above all else
1. `LinearIntervalTree`
    * Perfect for small, throw-away trees

Operation complexities:

| Operation |             Light/Quick |                 Linear |
|-----------|------------------------:|-----------------------:|
| Add       | 1 &nbsp;<sup>(AB)</sup> | 1 &nbsp;<sup>(A)</sup> |
| Remove    |  n &nbsp;<sup>(B)</sup> |                      n |
| Build     |              n * log(n) |                      - |
| Query     |              k + log(n) |                      n |
| Clear     |                       1 |                      1 |

n: number of intervals<br>
k: number of returned/altered intervals<br>
A: amortized<br>
B: requires build before next query

### LightIntervalTree

This class is all about memory efficiency. It implements an [Augmented Interval Tree (Wikipedia)](https://en.wikipedia.org/wiki/Interval_tree#Augmented_tree) which forms a simple binary search tree from the intervals and only requires storing one extra property (a subtree max-value) with each interval.

The simplicity of this tree makes it light and quick to initialise, but querying the tree requires a lot of key-comparisons, especially if intervals are densely packed and overlap to a high degree.

This tree is balanced on the first query. Adding/removing intervals causes the tree to re-initialise again on the next query.

### QuickIntervalTree

This class trades a small amount of memory efficiency in favour of significantly faster queries - about a 20% increase. It is an implementation of a [Centered Interval Tree (Wikipedia)](https://en.wikipedia.org/wiki/Interval_tree#Centered_interval_tree). This is the same datastructure that [RangeTree (GitHub)](https://github.com/mbuchetics/RangeTree) implements.

This datastructure requires building a search-tree separate from the intervals, which requires additional memory and initialisation time. The benefit is that far fewer key-comparison are required when querying the tree, especially in cases where intervals overlap.

This tree is balanced on the first query. Adding/removing intervals causes the tree to re-initialise again on the next query.

### LinearIntervalTree

For small dataset where only a few queries are needed, it may be faster not to build a tree at all. This class simply implements the familiar query methods over an unordered array. When queried, the whole array is checked for matches.

Due to it's simplicity, no "build" process or additional allocations are required before being queryable. Similarly, no re-build is required after any additions/removals from the "tree".

If your number of intervals (n) and number of queries (m) satisfy; n&nbsp;*&nbsp;m&nbsp;<&nbsp;10_000, then you may find this class to be your best performer in all aspects.

## Limitations

Please see the section on [Thread Safety](#thread-safety).

`LightIntervalTree` and `QuickIntervalTree` are limited to approximately 2 billion intervals. This is because `int`s are used as "pointers" as an optimization. Storing 2 billion intervals would take approximately 50GB~100GB of memory, so this limitation is mostly theoretical.

## Thread Safety

Tree-initialization, triggered by the **first query invocation, is _not_ thread safe**.

Subsequent concurrent queries _are_ thread safe.

Any modifications, adding/clearing/removing intervals, require exclusive access, followed by a single query to re-initialise the tree before releasing exclusive access.

It is up to the consumer to enforce synchronization controls.
Consider using something like [ReaderWriterLockSlim (Microsoft)](https://docs.microsoft.com/en-us/dotnet/api/system.threading.readerwriterlockslim).

To reduce the risk of significant problems stemming from misuse, a lock statement has been added to block concurrent initializations. This should prevent issues in cases where a tree is not initialised before being queried concurrently, however, this safety is is not guaranteed. Please take care to follow the advice above.


## TODOs

* Add query/find methods that return not just the value, but also the from and to of the matching intervals
* Play with INumber\<T\> and SIMD optimizations
* Add a dynamic tree, ie. one that does not require a rebuild after modifications
* Consider adding a RemoveAt(i)/Remove(from,to\[,value\]) method for ability to remove specific intervals


## Optimizations over RangeTree

A few key design decisions were made to reduce the memory usage.

1. Avoid keeping duplicate data

    `RangeTree` keeps a full, unused copy of intervals, in case the tree needs to be rebuilt following the addition or removal of an interval. This wastes memory.
    `LightIntervalTree` only stores intervals once, embedding tree information directly into the stored intervals. `QuickIntervalTree` directly uses the stored intervals, but also duplicates part of the intervals in order to store a reverse-order, needed to optimize searching.

1. Model tree nodes as value types (`struct`) rather than objects (`class`)

    Objects suffer memory overhead in the form of type and method information. Since `struct`s cannot reference their own type (to form a tree) an index (`int`) is used to reference other nodes by index.

1. Store nodes and intervals in indexable arrays, use indexes rather than references as pointers

    Pointers in 64-bit systems take up 8 bytes of storage, `int`s only take 4 bytes. Storing value types in Lists/Arrays improves CPU caching since elements are co-located in memory.

1. Nodes reference their intervals by index and length

    `RangeTree` allocates an array for each node to store intervals in. This project keeps all intervals in a single array. All related intervals are grouped, and each node keeps an index and count to point to the related intervals. This approach eliminates the overhead of small array allocations.

1. Iterative searching

    `RangeTree`, as well as early versions of this project, uses recursion to search smaller and smaller subtrees, eventually propagating results back up to the initial caller. Each method call, however, incurs some overhead from pushing the same arguments to the stack repeatedly. Newer version of this project use an iterative depth-first-search algorithm, backed by a small stack-allocated buffer for tracking progress. This speeds up querying without adding any heap allocations.
