# Changelog

## v0.2.0 - 2022/04/22

Added:
* Property `Values` to allow enumeration of all current values
* `IIntervalTree<TKey,TValue>` now implements `IEnumerable<Interval<TKey,TValue>>`

## v0.1.0 - 2022/04/20

First public release.

Features limited to `Add(from, to, value)` and `Query(x)`.