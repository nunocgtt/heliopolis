using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities.SpatialTreeIndexSystem
{
    [Serializable]
    public class SpatialTreeNode
    {
        private readonly SpatialTreeNode _parent;
        private readonly SpatialTreeIndex _spatialTreeIndex;
        public readonly SortedDictionary<string, int> ResourceCount = new SortedDictionary<string, int>();
        public SortedDictionary<int, SpatialTreeNode> Children { get; private set; }
        public Point TopLeft { get; private set; }
        public Point BottomRight { get; private set; }
        public int Level { get; private set; }
        public int MaxLevel { get; private set; }

        public SpatialTreeNode(Point topLeft, Point bottomRight, int level, int maxLevel, SpatialTreeNode parent, SpatialTreeIndex spatialTreeIndex)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Level = level;
            MaxLevel = maxLevel;
            _parent = parent;
            _spatialTreeIndex = spatialTreeIndex;
        }

        public void ChangeResourceCount(string resourceName, int difference)
        {
            if (!ResourceCount.ContainsKey(resourceName))
            {
                ResourceCount.Add(resourceName, 0);
            }
            ResourceCount[resourceName] += difference;
            if (_parent != null)
                _parent.ChangeResourceCount(resourceName, difference);
        }

        public void Construct()
        {
            int treeWidth = _spatialTreeIndex.TreeWidth[Level];
            int newWidth = ((BottomRight.X - TopLeft.X) + 1) / treeWidth;
            int newHeight = ((BottomRight.Y - TopLeft.Y) + 1) / treeWidth;
            if (Level < MaxLevel)
            {
                Children = new SortedDictionary<int, SpatialTreeNode>();
                for (int i = 0; i < treeWidth; i++)
                {
                    for (int j = 0; j < treeWidth; j++)
                    {
                        Point newTopLeft = new Point(TopLeft.X + (newWidth * i), TopLeft.Y + (newHeight * j));
                        Point newBottomRight = new Point(newTopLeft.X + newWidth - 1, newTopLeft.Y + newHeight - 1);
                        SpatialTreeNode addNode = new SpatialTreeNode(newTopLeft, newBottomRight, Level + 1, MaxLevel, this, _spatialTreeIndex);
                        Children.Add(SpatialTreeIndex.PointToIndex(new Point(i, j), treeWidth), addNode);
                        addNode.Construct();
                    }
                }
            }
            else
            {
                Children = null;
                for (int i = TopLeft.X; i <= BottomRight.X; i++)
                {
                    for (int j = TopLeft.Y; j <= BottomRight.Y; j++)
                    {
                        // Link this particular tile into this leaf node.
                        _spatialTreeIndex.GridToNodes[i, j] = this;
                    }
                }
            }
        }
    }
}
