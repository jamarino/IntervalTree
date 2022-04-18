using System.Numerics;

namespace LightIntervalTree
{
    public class LargeSparseIntervalTree<TKey, TValue> : IIntervalTree<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        protected readonly List<Node> _nodes = new();
        protected readonly List<Interval> _intervals = new() { new Interval() };
        private IComparer<TKey> _comparer = Comparer<TKey>.Default;

        public int NodeCount => _nodes.Count;

        public int Count { get; private set; }

        public int TreeDepth { get; private set; }

        public void Add(TKey from, TKey to, TValue value)
        {
            Count++;

            if (_nodes.Count == 0)
            {
                // insert root
                _nodes.Add(new Node
                {
                    Center = from,
                    LeftNodeIndex = -1,
                    RightNodeIndex = -1,
                    Interval = _intervals.Count
                });

                _intervals.Add(new Interval
                {
                    From = from,
                    To = to,
                    Value = value
                });

                TreeDepth = 1;
                return;
            }

            RecursiveAdd(0, 1, from, to, value);

            if (TreeDepth > 2 * BitOperations.Log2((uint)_nodes.Count))
                Optimize();
        }

        private void RecursiveAdd(int nodeIndex, int depth, TKey from, TKey to, TValue value)
        {
            var node = nodeIndex < _nodes.Count ? _nodes[nodeIndex] : default;

            if (node == default)
            {
                // new node
                node.Center = from;
                node.LeftNodeIndex = -1;
                node.RightNodeIndex = -1;
                node.Interval = _intervals.Count;
                _nodes.Add(node);

                _intervals.Add(new Interval
                {
                    From = from,
                    To = to,
                    Value = value
                });

                TreeDepth = Math.Max(TreeDepth, depth);
                return;
            }

            var compareTo = _comparer.Compare(to, node.Center);
            var compareFrom = _comparer.Compare(from, node.Center);

            if (compareTo < 0)
            {
                // recurse left
                var leftNodeIndex = node.LeftNodeIndex;
                if (leftNodeIndex == -1)
                {
                    leftNodeIndex = _nodes.Count;
                    node.LeftNodeIndex = leftNodeIndex;
                }

                RecursiveAdd(leftNodeIndex, depth+1, from, to, value);
            }
            else if (compareFrom > 0)
            {
                // recurse right
                var rightNodeIndex = node.RightNodeIndex;
                if (rightNodeIndex == -1)
                {
                    rightNodeIndex = _nodes.Count;
                    node.RightNodeIndex = rightNodeIndex;
                }

                RecursiveAdd(rightNodeIndex, depth+1, from, to, value);
            }
            else
            {
                // add to current node
                // TODO insert in ascending order
                var intervalIndex = node.Interval;
                var interval = _intervals[intervalIndex];

                while (true)
                {
                    if (interval.Next is 0) break;
                    intervalIndex = interval.Next;
                    interval = _intervals[intervalIndex];
                }

                _intervals.Add(new Interval
                {
                    From = from,
                    To = to,
                    Value = value
                });

                _intervals[intervalIndex] = interval with
                {
                    Next = _intervals.Count - 1
                };
            }

            _nodes[nodeIndex] = node;
        }

        public IEnumerable<TValue> Query(TKey target)
        {
            if (_nodes.Count is 0) return Enumerable.Empty<TValue>();

            var results = new List<TValue>();

            var node = _nodes[0];
            while (true)
            {
                var interval = _intervals[node.Interval];
                while (true)
                {
                    if (_comparer.Compare(target, interval.From) >= 0 &&
                        _comparer.Compare(target, interval.To) <= 0)
                    {
                        // target is in interval
                        results.Add(interval.Value);
                    }
                    if (interval.Next is 0) break;
                    interval = _intervals[interval.Next];
                }

                if (_comparer.Compare(target, node.Center) == 0) break;
                else if (_comparer.Compare(target, node.Center) < 0)
                {
                    // left
                    if (node.LeftNodeIndex == -1) break;
                    node = _nodes[node.LeftNodeIndex];
                }
                else
                {
                    // right
                    if (node.RightNodeIndex == -1) break;
                    node = _nodes[node.RightNodeIndex];
                }
            }

            return results;
        }

        public void Optimize()
        {
            (int, int) OptRec(int rootIndex)
            {
                if (rootIndex is -1) return (-1, -1);

                var root = _nodes[rootIndex];
                var leftInfo = OptRec(root.LeftNodeIndex);
                var rightInfo = OptRec(root.RightNodeIndex);

                var leftDepth = Math.Max(leftInfo.Item1, leftInfo.Item2) + 1;
                var rightDepth = Math.Max(rightInfo.Item1, rightInfo.Item2) + 1;

                if (leftDepth - rightDepth > 1)
                {
                    // left is deeper
                    if (leftInfo.Item1 < leftInfo.Item2)
                    {
                        // need to pull up deeper subtree
                        //PullUpLeft(rootIndex);
                        RotateLeft(root.LeftNodeIndex);
                        RotateRight(rootIndex);
                        leftDepth--;
                        rightDepth++;
                    }
                    else
                    {
                        RotateRight(rootIndex);
                        leftDepth--;
                        rightDepth = 1 + leftInfo.Item2;
                    }
                }
                else if (rightDepth - leftDepth > 1)
                {
                    // right is deeper
                    if (rightInfo.Item1 > rightInfo.Item2)
                    {
                        // need to pull up deeper subtree
                        //PullUpRight(rootIndex);
                        RotateRight(root.RightNodeIndex);
                        RotateLeft(rootIndex);
                        rightDepth--;
                        leftDepth++;
                    }
                    else
                    {
                        RotateLeft(rootIndex);
                        rightDepth--;
                        leftDepth = 1 + rightInfo.Item1;
                    }
                }

                return (leftDepth, rightDepth);
            }

            var (l, r) = OptRec(0);
            TreeDepth = Math.Max(l, r) + 1;
        }

        private void RotateRight(int nodeIndex)
        {
            var node = _nodes[nodeIndex];
            var leftNode = _nodes[node.LeftNodeIndex];

            var newRightNode = node with
            {
                LeftNodeIndex = leftNode.RightNodeIndex
            };

            var newRoot = leftNode with
            {
                RightNodeIndex = node.LeftNodeIndex
            };

            var prevIntervalIndex = 0;
            var prevInterval = new Interval();
            var intervalIndex = newRightNode.Interval;
            while (intervalIndex is not 0)
            {
                var interval = _intervals[intervalIndex];
                var next = interval.Next;
                if (_comparer.Compare(interval.From, newRoot.Center) <= 0 &&
                    _comparer.Compare(interval.To, newRoot.Center) >= 0)
                {
                    // interval must be moved to new root
                    // remove interval from old chain
                    if (prevIntervalIndex is 0)
                    {
                        // this is the first interval in the chain, must update node
                        newRightNode.Interval = interval.Next;
                    }
                    else
                    {
                        // this is not the first interval, must update previous interval
                        prevInterval.Next = interval.Next;
                        _intervals[prevIntervalIndex] = prevInterval;
                    }

                    // add interval into newRoot chain
                    interval.Next = newRoot.Interval;
                    _intervals[intervalIndex] = interval;
                    newRoot.Interval = intervalIndex;
                }
                else
                {
                    prevIntervalIndex = intervalIndex;
                    prevInterval = interval;
                }
                
                intervalIndex = next;
            }

            _nodes[nodeIndex] = newRoot;
            _nodes[node.LeftNodeIndex] = newRightNode;
        }

        private void RotateLeft(int nodeIndex)
        {
            var node = _nodes[nodeIndex];
            var rightNode = _nodes[node.RightNodeIndex];

            var newLeftNode = node with
            {
                RightNodeIndex = rightNode.LeftNodeIndex
            };

            var newRoot = rightNode with
            {
                LeftNodeIndex = node.RightNodeIndex
            };

            var prevIntervalIndex = 0;
            var prevInterval = new Interval();
            var intervalIndex = newLeftNode.Interval;
            while (intervalIndex is not 0)
            {
                var interval = _intervals[intervalIndex];
                var next = interval.Next;
                if (_comparer.Compare(interval.From, newRoot.Center) <= 0 &&
                    _comparer.Compare(interval.To, newRoot.Center) >= 0)
                {
                    // interval must be moved to new root
                    // remove interval from old chain
                    if (prevIntervalIndex is 0)
                    {
                        // this is the first interval in the chain, must update node
                        newLeftNode.Interval = interval.Next;
                    }
                    else
                    {
                        // this is not the first interval, must update previous interval
                        prevInterval.Next = interval.Next;
                        _intervals[prevIntervalIndex] = prevInterval;
                    }

                    // add interval into newRoot chain
                    interval.Next = newRoot.Interval;
                    _intervals[intervalIndex] = interval;
                    newRoot.Interval = intervalIndex;
                }
                else
                {
                    prevIntervalIndex = intervalIndex;
                    prevInterval = interval;
                }

                intervalIndex = next;
            }

            _nodes[nodeIndex] = newRoot;
            _nodes[node.RightNodeIndex] = newLeftNode;
        }

        protected record struct Node
        {
            public TKey Center;
            public int Interval;
            public int LeftNodeIndex;
            public int RightNodeIndex;
        }

        protected record struct Interval
        {
            public TKey From;
            public TKey To;
            public TValue Value;
            public int Next;
        }
    }
}