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

## TODO list

* Order intervals within nodes to optimize queries
* Add properties for enumerating values and intervals
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