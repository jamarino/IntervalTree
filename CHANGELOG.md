# Changelog

## v0.7.0 - 2023/07/18

Query performance of LightIntervalTree improved by a further 15% in dense benchmarks, by switching to a naive linear scan of intervals when subtree size falls to just a handful of intervals.

## v0.6.0 - 2023/07/18

Query performance improved by 10-35% by replacing recursive query methods with allocation-free depth-first-search iterative implementation.

Note: netstandard2.0 now requires System.Memory for Span<>

## v0.5.0 - 2023/07/12

Added a type constraint on `TKey`, the 'from' and 'to components of the intervals, so that it must implement `IComparable<TKey>`.

Most obvious types, int, double, decimal, long, etc. still work with this constraint, and query performance improved by 20-30%.

## v0.4.0 - 2023/07/10

Renamed project from LightIntervalTree to Jamarino.IntervalTree

## v0.3.0 - 2022/05/05

Added:
* New class `QuickIntervalTree` was added
    * This class implements a centered interval tree, see README for more information on tree differences
* Added NuGet description, project URL, tags and more

Changed:
* `LightIntervalTree` was re-implemented as an Augmented Tree
    * See README for more information on tree differences
    * This tree no longer auto-balances when intervals are added, but instead re-builds on first query after a change

## v0.2.0 - 2022/04/22

Added:
* Property `Values` to allow enumeration of all current values
* `IIntervalTree<TKey,TValue>` now implements `IEnumerable<Interval<TKey,TValue>>`

## v0.1.0 - 2022/04/20

First public release.

Features limited to `Add(from, to, value)` and `Query(x)`.