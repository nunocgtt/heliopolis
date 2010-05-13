using System;
using System.Collections.Generic;
using System.Linq;
using Heliopolis.World;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities.SpatialTreeIndexSystem
{
    [Serializable]
    public class SpatialTreeNode
    {
        private SpatialTreeNode _parent;
        private SpatialTreeIndex _spatialTreeIndex;
        public readonly SortedDictionary<SpatialObjectKey, int> ResourceCount = new SortedDictionary<SpatialObjectKey, int>();
        public SortedDictionary<Point, SpatialTreeNode> Children { get; private set; }
        public Point TopLeft { get; private set; }
        public Point BottomRight { get; private set; }
        public int Level { get; private set; }
        public int MaxLevel { get; private set; }

        public Dictionary<SpatialObjectKey, List<ISpatialIndexMember>> SpatialIndexMembers { get; set; }

        public bool IsLeafNode
        {
            get
            {
                return Children.Count == 0;
            } 
        }

        private SpatialTreeNode(Point topLeft, Point bottomRight, int level, int maxLevel, SpatialTreeNode parent, SpatialTreeIndex spatialTreeIndex)
        {
            Init(topLeft, bottomRight, level, maxLevel, parent, spatialTreeIndex);
        }

        public SpatialTreeNode(Point topLeft, Point bottomRight, int level, int maxLevel, SpatialTreeIndex spatialTreeIndex)
        {
            Init(topLeft, bottomRight, level, maxLevel, null, spatialTreeIndex);
        }

        private void Init(Point topLeft, Point bottomRight, int level, int maxLevel, SpatialTreeNode parent, SpatialTreeIndex spatialTreeIndex)
        {
            SpatialIndexMembers = new Dictionary<SpatialObjectKey, List<ISpatialIndexMember>>();
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Level = level;
            MaxLevel = maxLevel;
            _parent = parent;
            _spatialTreeIndex = spatialTreeIndex;
        }


        public void AddSpatialMemeber(SpatialObjectKey resourceName, ISpatialIndexMember member)
        {
            if (IsLeafNode)
            {
                if (SpatialIndexMembers[resourceName] == null)
                {
                    SpatialIndexMembers[resourceName] = new List<ISpatialIndexMember>();
                }
                SpatialIndexMembers[resourceName].Add(member);
                ChangeResourceCount(resourceName, 1);
            }
        }

        public ISpatialIndexMember GetResource(SpatialObjectKey resourceName)
        {
            if (IsLeafNode && SpatialIndexMembers[resourceName] != null)
            {
                return SpatialIndexMembers[resourceName].First();
            }
            throw new ItemNotFound("Item not found.");
        }

        public void RemoveSpatialMemeber(SpatialObjectKey resourceName, ISpatialIndexMember member)
        {
            if (IsLeafNode && SpatialIndexMembers[resourceName] != null)
            {
                if (SpatialIndexMembers[resourceName].Contains(member))
                {
                    SpatialIndexMembers[resourceName].Remove(member);
                    ChangeResourceCount(resourceName, -1);
                }
            }
        }

        private void ChangeResourceCount(SpatialObjectKey resourceName, int difference)
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
            int treeHeight = _spatialTreeIndex.TreeHeight[Level];
            int subWidth = (int)Math.Ceiling(((BottomRight.X - TopLeft.X) + 1) / (double)treeWidth);
            int newHeight = (int)Math.Ceiling(((BottomRight.Y - TopLeft.Y) + 1) / (double)treeHeight);
            if (Level < MaxLevel)
            {
                Children = new SortedDictionary<Point, SpatialTreeNode>();
                for (int i = 0; i < treeWidth; i++)
                {
                    for (int j = 0; j < treeHeight; j++)
                    {
                        Point newTopLeft = new Point(TopLeft.X + (subWidth * i), TopLeft.Y + (newHeight * j));
                        Point newBottomRight = new Point(Math.Min(newTopLeft.X + subWidth - 1, _spatialTreeIndex.WorldSize.X)
                            , Math.Min(newTopLeft.Y + newHeight - 1,_spatialTreeIndex.WorldSize.Y));
                        SpatialTreeNode addNode = new SpatialTreeNode(newTopLeft, newBottomRight, Level + 1, MaxLevel, this, _spatialTreeIndex);
                        Children.Add(new Point(i, j), addNode);
                        addNode.Construct();
                    }
                }
            }
            if (IsLeafNode)
            {
                for (int i = TopLeft.X; i <= BottomRight.X; i++)
                {
                    for (int j = TopLeft.Y; j <= BottomRight.Y; j++)
                    {
                        // Link this particular tile into this leaf node.
                        _spatialTreeIndex.GridToLeafNodes[i, j] = this;
                    }
                }
            }
        }
    }
}
