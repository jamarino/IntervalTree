using System;
using Jamarino.IntervalTree;

namespace Extras;

public static class TreeFactory
{
    public static string[] TreeTypes = new string[] {
        "reference",
        "light",
        "quick"
    };

    public static string[] TreeTypesSansReference = new string[] {
        "light",
        "quick"
    };

    public static IntervalTree.IIntervalTree<TKey, TValue> CreateEmptyTree<TKey, TValue>(string type)
        where TKey : IComparable<TKey> 
        => type switch
        {
            "reference" => new IntervalTree.IntervalTree<TKey, TValue>(),
            "light" => new TreeAdapter<TKey, TValue>(new LightIntervalTree<TKey, TValue>()),
            "quick" => new TreeAdapter<TKey, TValue>(new QuickIntervalTree<TKey, TValue>()),
            _ => throw new ArgumentException($"Unkown tree type: {type}", nameof(type))
        };
}
