using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Jamarino.IntervalTree;

public static class FindableTreeExtensions
{
    public static TState Find<TKey, TValue, TState>(
        this IFindableTree<TKey, TValue> tree,
        TKey target,
        TState initialState,
        Func<Interval<TKey, TValue>, TState, TState> accumulator)
        where TKey : IComparable<TKey>
    {
        return tree.Find(target, target, initialState, accumulator);
    }

    public static List<Interval<TKey, TValue>> FindList<TKey, TValue>(
        this IFindableTree<TKey, TValue> tree,
        TKey target,
        List<Interval<TKey, TValue>>? existingList = null)
        where TKey : IComparable<TKey>
    {
        return tree.Find(
            target,
            target,
            existingList ?? new List<Interval<TKey, TValue>>(),
            static (interval, state) =>
            {
                state.Add(interval);
                return state;
            });
    }

    public static List<Interval<TKey, TValue>> FindList<TKey, TValue>(
        this IFindableTree<TKey, TValue> tree,
        TKey low,
        TKey high,
        List<Interval<TKey, TValue>>? existingList = null)
        where TKey : IComparable<TKey>
    {
        return tree.Find(
            low,
            high,
            existingList ?? new List<Interval<TKey, TValue>>(),
            static (interval, state) =>
            {
                state.Add(interval);
                return state;
            });
    }
}
