# Changelog

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