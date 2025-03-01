using System;
using System.Collections.Generic;
using System.Linq;
using Jamarino.IntervalTree;

namespace Extras;

public static class TreeFactory
{
    public static IEnumerable<string> TreeTypes = new string[] {
        "reference",
        "linear",
        "light",
        "quick",
    };

    public static IEnumerable<string> TreeTypesSansReference = TreeTypes.Where(t => t is not "reference");

    public static IntervalTree.IIntervalTree<TKey, TValue> CreateEmptyTree<TKey, TValue>(string type, int? capacity = null) where TKey : IComparable<TKey>
    {
        var tree = CreateEmptyTreeRaw<TKey, TValue>(type, capacity);

        if (type is "reference")
            return (IntervalTree.IntervalTree<TKey, TValue>)tree;

        return new TreeAdapter<TKey, TValue>((IIntervalTree<TKey, TValue>)tree);
    }

    public static IIntervalTree<TKey, TValue> CreateNonReferenceTree<TKey, TValue>(string type, int? capacity = null)
        where TKey : IComparable<TKey>
    {
        return (IIntervalTree<TKey, TValue>)CreateEmptyTreeRaw<TKey, TValue>(type, capacity);
    }

    public static object CreateEmptyTreeRaw<TKey, TValue>(string type, int? capacity = null) where TKey : IComparable<TKey>
    { 
        if (capacity is null)
        {
            return type switch
            {
                "reference" => new IntervalTree.IntervalTree<TKey, TValue>(),
                "linear" => new LinearIntervalTree<TKey, TValue>(),
                "light" => new LightIntervalTree<TKey, TValue>(),
                "quick" => new QuickIntervalTree<TKey, TValue>(),
                _ => throw new ArgumentException($"Unkown tree type: {type}", nameof(type))
            };
        }

        return type switch
        {
            "reference" => new IntervalTree.IntervalTree<TKey, TValue>(),
            "linear" => new LinearIntervalTree<TKey, TValue>(capacity),
            "light" => new LightIntervalTree<TKey, TValue>(capacity),
            "quick" => new QuickIntervalTree<TKey, TValue>(capacity),
            _ => throw new ArgumentException($"Unkown tree type: {type}", nameof(type))
        };
    }
}
