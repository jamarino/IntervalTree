using System;
using Jamarino.IntervalTree;

namespace Extras;

public static class TreeFactory
{
    public static string[] TreeTypes = new string[] {
        "reference",
        "light",
        "quick",
    };

    public static string[] TreeTypesSansReference = new string[] {
        "light",
        "quick",
    };

    public static IntervalTree.IIntervalTree<TKey, TValue> CreateEmptyTree<TKey, TValue>(string type, int? capacity = null) where TKey : IComparable<TKey>
    {
        if (capacity is not null)
            return type switch
            {
                "reference" => new IntervalTree.IntervalTree<TKey, TValue>(),
                "light" => new TreeAdapter<TKey, TValue>(new LightIntervalTree<TKey, TValue>(capacity)),
                "quick" => new TreeAdapter<TKey, TValue>(new QuickIntervalTree<TKey, TValue>(capacity)),
                _ => throw new ArgumentException($"Unkown tree type: {type}", nameof(type))
            };

        return type switch
        {
            "reference" => new IntervalTree.IntervalTree<TKey, TValue>(),
            "light" => new TreeAdapter<TKey, TValue>(new LightIntervalTree<TKey, TValue>()),
            "quick" => new TreeAdapter<TKey, TValue>(new QuickIntervalTree<TKey, TValue>()),
            _ => throw new ArgumentException($"Unkown tree type: {type}", nameof(type))
        };
    }
}
