using System;
using System.Collections.Generic;
using System.Linq;
using LightIntervalTree;

namespace Extras;

/// <summary>
/// Useful for debugging specific tree examples
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class LargeSparseIntervalTreeLongKey<TValue> : LargeSparseIntervalTree<long, TValue>
{
#if DEBUG
    public string FormatTree()
    {
        List<string> MapRec(int rootIndex)
        {
            if (rootIndex is -1) return new List<string>();

            var root = _nodes[rootIndex];
            var left = MapRec(root.LeftNodeIndex);
            var right = MapRec(root.RightNodeIndex);

            var leftWidth = left.Count is 0 ? 0 : left.Max(s => s.Length);
            var rightWidth = right.Count is 0 ? 0 : right.Max(s => s.Length);

            var rootString = $"{rootIndex}";
            var rootLine = $" {new string(' ', leftWidth)}{rootString}{new string(' ', rightWidth)} ";
            var lines = new List<string>() { rootLine };

            for (var i = 0; i < Math.Max(left.Count, right.Count); i++)
            {
                var l = i < left.Count ? left[i] : new string(' ', leftWidth);
                var r = i < right.Count ? right[i] : new string(' ', rightWidth);
                lines.Add($" {l}{new string(' ', rootString.Length)}{r} ");
            }

            return lines;
        }

        var ls = MapRec(0);
        return string.Join(Environment.NewLine, ls);
    }

    public string FormatIntervals()
    {
        var lines = new List<string>();
        var nodeQueue = new Queue<int>();
        nodeQueue.Enqueue(0);

        while (nodeQueue.Any())
        {
            var nodeIndex = nodeQueue.Dequeue();
            var node = _nodes[nodeIndex];
            if (node.LeftNodeIndex is not -1) nodeQueue.Enqueue(node.LeftNodeIndex);
            if (node.RightNodeIndex is not -1) nodeQueue.Enqueue(node.RightNodeIndex);

            lines.Add($"{nodeIndex,3}:{new string(' ', (int)node.Center)}O");

            var intervalIndex = node.Interval;
            while (intervalIndex is not 0)
            {
                var interval = _intervals[intervalIndex];
                intervalIndex = interval.Next;
                var intervalString = (interval.To - interval.From) switch
                {
                    1 => "X",
                    2 => "XX",
                    var len => $"X{new string('-', (int)len - 2)}X"
                };
                var line = $"    {new string(' ', (int)interval.From)}{intervalString}";
                lines.Add(line);
            }
        }

        return string.Join(Environment.NewLine, lines);
    }
#endif
}
