using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities
{
    [Serializable]
    public class SpatialTreeNode
    {
        private Point topLeft;
        private Point bottomRight;
        private int level;
        private int maxLevel;
        private SortedDictionary<int, SpatialTreeNode> children;
        private SpatialTreeNode parent;
        private SpatialTreeIndex spatialTreeIndex;

        public SortedDictionary<string, int> ResourceCount = new SortedDictionary<string, int>();

        public SortedDictionary<int, SpatialTreeNode> Children
        {
            get { return children; }
        }

        public SpatialTreeNode Parent
        {
            get { return parent; }
        }

        public Point TopLeft
        {
            get { return topLeft; }
            set { topLeft = value; }
        }

        public Point BottomRight
        {
            get { return bottomRight; }
            set { bottomRight = value; }
        }

        public int Level
        {
            get { return level; }
        }

        public int MaxLevel
        {
            get { return maxLevel; }
        }

        public SpatialTreeNode(Point _topLeft, Point _bottomRight, int _level, int _maxLevel, SpatialTreeNode _parent, SpatialTreeIndex _spatialTreeIndex)
        {
            topLeft = _topLeft;
            bottomRight = _bottomRight;
            level = _level;
            maxLevel = _maxLevel;
            parent = _parent;
            spatialTreeIndex = _spatialTreeIndex;
        }

        public void ChangeResourceCount(string resourceName, int difference)
        {
            if (!ResourceCount.ContainsKey(resourceName))
            {
                ResourceCount.Add(resourceName, 0);
            }
            ResourceCount[resourceName] += difference;
            if (parent != null)
                parent.ChangeResourceCount(resourceName, difference);
        }

        public void Construct()
        {
            int treeWidth = spatialTreeIndex.TreeWidth[level];
            int newWidth = ((bottomRight.X - topLeft.X) + 1) / treeWidth;
            int newHeight = ((bottomRight.Y - topLeft.Y) + 1) / treeWidth;
            if (level < maxLevel)
            {
                children = new SortedDictionary<int, SpatialTreeNode>();
                for (int i = 0; i < treeWidth; i++)
                {
                    for (int j = 0; j < treeWidth; j++)
                    {
                        Point newTopLeft = new Point(topLeft.X + (newWidth * i), topLeft.Y + (newHeight * j));
                        Point newBottomRight = new Point(newTopLeft.X + newWidth - 1, newTopLeft.Y + newHeight - 1);
                        SpatialTreeNode addNode = new SpatialTreeNode(newTopLeft, newBottomRight, level + 1, maxLevel, this, spatialTreeIndex);
                        children.Add(SpatialTreeIndex.PointToIndex(new Point(i, j), treeWidth), addNode);
                        addNode.Construct();
                    }
                }
            }
            else
            {
                children = null;
                for (int i = topLeft.X; i <= bottomRight.X; i++)
                {
                    for (int j = topLeft.Y; j <= bottomRight.Y; j++)
                    {
                        // Link this particular tile into this leaf node.
                        spatialTreeIndex.GridToNodes[i, j] = this;
                    }
                }
            }
        }
    }
}
