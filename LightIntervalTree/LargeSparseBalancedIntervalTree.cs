namespace LightIntervalTree;

public class LargeSparseBalancedIntervalTree<TKey, TValue> : IIntervalTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    protected readonly List<Node> _nodes = new();
    protected readonly List<Interval> _intervals = new() { new Interval() };
    private IComparer<TKey> _comparer = Comparer<TKey>.Default;

    public int NodeCount => _nodes.Count;

    public int Count { get; private set; }

    public void Add(TKey from, TKey to, TValue value)
    {
        Count++;

        RecursiveAdd(0, 1, from, to, value);
    }

    private bool RecursiveAdd(int nodeIndex, int depth, TKey from, TKey to, TValue value)
    {
        if (nodeIndex >= _nodes.Count)
        {
            // new node
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
            return true;
        }

        var node = _nodes[nodeIndex];
        var initialNodeBalance = node.Balance;

        var compareTo = _comparer.Compare(to, node.Center);
        var compareFrom = _comparer.Compare(from, node.Center);

        if (compareTo < 0)
        {
            // recurse left
            if (node.LeftNodeIndex == -1)
            {
                node.LeftNodeIndex = _nodes.Count;
            }

            var heightIncreased = RecursiveAdd(node.LeftNodeIndex, depth+1, from, to, value);
            if (heightIncreased)
                node.Balance--;
        }
        else if (compareFrom > 0)
        {
            // recurse right
            if (node.RightNodeIndex == -1)
            {
                node.RightNodeIndex = _nodes.Count;
            }

            var heightIncreased = RecursiveAdd(node.RightNodeIndex, depth+1, from, to, value);
            if (heightIncreased)
                node.Balance++;
        }
        else
        {
            // add to current node
            // TODO insert in ascending order
            _intervals.Add(new Interval
            {
                From = from,
                To = to,
                Value = value,
                Next = node.Interval
            });
            node.Interval = _intervals.Count - 1;
        }

        // update node
        _nodes[nodeIndex] = node;

        // balance
        if (node.Balance < -1)
        {
            // left is too deep
            var left = _nodes[node.LeftNodeIndex];
            if (left.Balance <= 0)
            {
                // subtree is balanced or deeper on left side
                BalanceLeftLeft(nodeIndex);
            }
            else
            {
                // subtree is deeper on the right
                BalanceLeftRight(nodeIndex);
            }
        }
        else if (node.Balance > 1)
        {
            // right is too deep
            var right = _nodes[node.RightNodeIndex];
            if (right.Balance >= 0)
            {
                // subtree is balanced or deeper on right side
                BalanceRightRight(nodeIndex);
            }
            else
            {
                // subtree is deeper on the left
                BalanceRightLeft(nodeIndex);
            }
        }

        // reload node in case of rotations
        node = _nodes[nodeIndex];

        // return true if the node balance has changed and is not 0
        return node.Balance != initialNodeBalance && node.Balance is not 0;
    }

    private void BalanceRightRight(int rootIndex)
    {
        RotateLeft(rootIndex);

        // update balance
        // note that child is now on the left
        var root = _nodes[rootIndex];
        var child = _nodes[root.LeftNodeIndex];

        root.Balance = 0;
        child.Balance = 0;

        _nodes[rootIndex] = root;
        _nodes[root.LeftNodeIndex] = child;

        PromoteIntervals(rootIndex, root.LeftNodeIndex);
    }

    private void BalanceLeftLeft(int rootIndex)
    {
        RotateRight(rootIndex);

        // update balance
        // note that child is new on the right
        var root = _nodes[rootIndex];
        var child = _nodes[root.RightNodeIndex];

        root.Balance = 0;
        child.Balance = 0;

        _nodes[rootIndex] = root;
        _nodes[root.RightNodeIndex] = child;

        PromoteIntervals(rootIndex, root.RightNodeIndex);
    }

    private void BalanceRightLeft(int rootIndex)
    {
        // see: https://en.wikipedia.org/wiki/AVL_tree#:~:text=of%20rotated%20subtree%0A%7D-,Double%20rotation,-%5Bedit%5D

        var oldRoot = _nodes[rootIndex];
        RotateRight(oldRoot.RightNodeIndex);
        RotateLeft(rootIndex);

        var y = _nodes[rootIndex];
        var x = _nodes[y.LeftNodeIndex];
        var z = _nodes[y.RightNodeIndex];

        if (y.Balance > 0)
        {
            x.Balance = -1;
            z.Balance = 0;
        }
        else
        {
            x.Balance = 0;
            z.Balance = 1;
        }
        y.Balance = 0;

        _nodes[rootIndex] = y;
        _nodes[y.LeftNodeIndex] = x;
        _nodes[y.RightNodeIndex] = z;

        PromoteIntervals(rootIndex, y.RightNodeIndex);
        PromoteIntervals(rootIndex, y.LeftNodeIndex);
    }

    private void BalanceLeftRight(int rootIndex)
    {
        // see: https://en.wikipedia.org/wiki/AVL_tree#:~:text=of%20rotated%20subtree%0A%7D-,Double%20rotation,-%5Bedit%5D

        var oldRoot = _nodes[rootIndex];
        RotateLeft(oldRoot.LeftNodeIndex);
        RotateRight(rootIndex);

        var y = _nodes[rootIndex];
        var x = _nodes[y.RightNodeIndex];
        var z = _nodes[y.LeftNodeIndex];

        if (y.Balance < 0)
        {
            x.Balance = -1;
            z.Balance = 0;
        }
        else
        {
            x.Balance = 0;
            z.Balance = 1;
        }
        y.Balance = 0;

        _nodes[rootIndex] = y;
        _nodes[y.RightNodeIndex] = x;
        _nodes[y.LeftNodeIndex] = z;

        PromoteIntervals(rootIndex, y.RightNodeIndex);
        PromoteIntervals(rootIndex, y.LeftNodeIndex);
    }

    public IEnumerable<TValue> Query(TKey target)
    {
        if (_nodes.Count is 0) return Enumerable.Empty<TValue>();

        var results = new List<TValue>();

        var node = _nodes[0];
        while (true)
        {
            var intervalIndex = node.Interval;
            while (intervalIndex is not 0)
            {
                var interval = _intervals[intervalIndex];
                if (_comparer.Compare(target, interval.From) >= 0 &&
                    _comparer.Compare(target, interval.To) <= 0)
                {
                    // target is in interval
                    results.Add(interval.Value);
                }
                intervalIndex = interval.Next;
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

        _nodes[nodeIndex] = newRoot;
        _nodes[node.LeftNodeIndex] = newRightNode;

        PromoteIntervals(nodeIndex, node.LeftNodeIndex);
    }

    private void RotateLeft(int nodeIndex)
    {
        // create new nodes
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

        // TODO update balance of nodes

        // save new nodes
        _nodes[nodeIndex] = newRoot;
        _nodes[node.RightNodeIndex] = newLeftNode;

        PromoteIntervals(nodeIndex, node.RightNodeIndex);
    }

    private void PromoteIntervals(int newRootNodeIndex, int newChildNodeIndex)
    {
        var newRoot = _nodes[newRootNodeIndex];
        var newChild = _nodes[newChildNodeIndex];

        // move up intervals that overlap with new root node
        var prevIntervalIndex = 0;
        var prevInterval = new Interval();
        var intervalIndex = newChild.Interval;
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
                    newChild.Interval = interval.Next;
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

        _nodes[newRootNodeIndex] = newRoot;
        _nodes[newChildNodeIndex] = newChild;
    }

    protected record struct Node
    {
        public TKey Center;
        public int Interval;
        public int LeftNodeIndex;
        public int RightNodeIndex;
        public sbyte Balance;
    }

    protected record struct Interval
    {
        public TKey From;
        public TKey To;
        public TValue Value;
        public int Next;
    }
}
