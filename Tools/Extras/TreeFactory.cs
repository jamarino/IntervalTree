using System;
using Jamarino.IntervalTree;

namespace Extras;

public static class TreeFactory
{
    public static string[] TreeTypes = new string[] {
        "reference",
        "light",
        "lightopt",
        "quick",
    };

    public static string[] TreeTypesSansReference = new string[] {
        "light",
        "lightopt",
        "quick",
    };

    public static IntervalTree.IIntervalTree<TKey, TValue> CreateEmptyTree<TKey, TValue>(string type) where TKey : IComparable<TKey>
        => type switch
        {
            "reference" => new IntervalTree.IntervalTree<TKey, TValue>(),
            "light" => new TreeAdapter<TKey, TValue>(new LightIntervalTree<TKey, TValue>()),
            "lightopt" => new TreeAdapter<TKey, TValue>(new LightIntervalTreeOpt<TKey, TValue>()),
            "quick" => new TreeAdapter<TKey, TValue>(new QuickIntervalTree<TKey, TValue>()),
            _ => throw new ArgumentException($"Unkown tree type: {type}", nameof(type))
        };
}
