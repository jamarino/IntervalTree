using System;
using System.Collections.Generic;
using System.Text;

namespace Jamarino.IntervalTree;

public interface IFindableTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    /// <summary>
    /// Find all intervals overlapping the provided range.
    /// The accumulator action is called with each matched interval and accumulated state.
    /// The state produced is fed into the next execution of the accumulator, and ultimately returned.
    /// Note that the tree will first be built if required.
    /// </summary>
    /// <param name="low">The low end of the search interval (inclusive)</param>
    /// <param name="high">The high end of the search interval (inclusive)</param>
    /// <param name="initialState">The initial state to accumulate on. Example: new List&lt;TValue&gt;()</param>
    /// <param name="accumulator">Function to execute for each matched interval. A static lambda function is recommended.</param>
    /// <returns>Values associated with matching intervals</returns>
    TState Find<TState>(TKey low, TKey high, TState initialState, Func<Interval<TKey, TValue>, TState, TState> accumulator);
}
