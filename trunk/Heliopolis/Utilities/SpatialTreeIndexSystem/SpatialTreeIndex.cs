using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Heliopolis.World;

namespace Heliopolis.Utilities.SpatialTreeIndexSystem
{
    /// <summary>
    /// Represents a spatial index for fast retrieval of objects close to a point. Also manages object
    /// separation into "sections".
    /// </summary>
    [Serializable]
    public class SpatialTreeIndex 
    {
        public SpatialTreeIndex(Point worldSize, int [] treeWidth, int [] treeHeight)
        {
            WorldSize = worldSize;
            TreeWidth = treeWidth;
            TreeHeight = treeHeight;
        }

        public Point WorldSize { get; private set; }

        /// <summary>
        /// This number squared will give the number of branches at level
        /// </summary>
        public int[] TreeWidth { get; private set; }
        public int[] TreeHeight { get; private set; }
        /// <summary>
        /// A grid which provides quick access to an Environment Tile's relevant spatial tree node.
        /// </summary>
        public SpatialTreeNode[,] GridToLeafNodes;

        private SpatialTreeNode PositionToLeafNode(Point position)
        {
            return GridToLeafNodes[position.X, position.Y];
        }

        public void CheckChangeSection(Point oldPosition, Point newPosition, ISpatialIndexMember objectToMove, SpatialObjectKey spatialObjectType)
        {
            SpatialTreeNode leafNodeToRemoveFrom = PositionToLeafNode(oldPosition);
            SpatialTreeNode leafNodeToAdd = PositionToLeafNode(newPosition);
            if (leafNodeToRemoveFrom != leafNodeToAdd)
            {
                RemoveFromSection(oldPosition, objectToMove, spatialObjectType);               
                AddToSection(newPosition, objectToMove, spatialObjectType);
            }
        }

        public void AddToSection(Point position, ISpatialIndexMember objectToAdd, SpatialObjectKey spatialObjectType)
        {
            SpatialTreeNode leafNodeToAdd = PositionToLeafNode(position);
            leafNodeToAdd.AddSpatialMemeber(spatialObjectType, objectToAdd);
        }

        public void RemoveFromSection(Point position, ISpatialIndexMember objectToRemove, SpatialObjectKey spatialObjectType)
        {
            SpatialTreeNode leafNodeToRemoveFrom = PositionToLeafNode(position);
            leafNodeToRemoveFrom.RemoveSpatialMemeber(spatialObjectType, objectToRemove);
        }

        public void Initialise()
        {
            _topNode = new SpatialTreeNode(new Point(0, 0), new Point(WorldSize.X - 1, WorldSize.Y - 1), 0, TreeWidth.Length - 1, this);
            GridToLeafNodes = new SpatialTreeNode[WorldSize.X, WorldSize.Y];
            _topNode.Construct();
        }

        private SpatialTreeNode _topNode;

        public SpatialTreeNode TopNode
        {
            get { return _topNode; }
        }

        public ISpatialIndexMember FindClosestObject(Point searcherPoint, SpatialObjectKey objectType)
        {
            if (_topNode.ResourceCount.ContainsKey(objectType))
            {
                if (_topNode.ResourceCount[objectType] > 0)
                {
                    return SearchTree(searcherPoint, objectType);
                }
                throw new ItemNotFound("No item available");
            }
            throw new ItemNotFound("No item available");
        }

        private ISpatialIndexMember SearchTree(Point searcherPoint, SpatialObjectKey itemType)
        {
            List<SpatialTreeNode> startNodeList = new List<SpatialTreeNode> {_topNode};

            int level = _topNode.Level;
            int maxLevel = _topNode.MaxLevel;

            while (level < maxLevel)
            {
                int currentMinDistance = Int32.MaxValue;
                int currentMaxDistance = Int32.MaxValue;
                List<SpatialTreeNode> searchList = new List<SpatialTreeNode>();
                foreach (SpatialTreeNode startSpatialTreeNode in startNodeList)
                {
                    foreach (SpatialTreeNode spatialTreeNode in startSpatialTreeNode.Children)
                    {
                        if (spatialTreeNode.ResourceCount.ContainsKey(itemType))
                            if (spatialTreeNode.ResourceCount[itemType] > 0)
                            {
                                int minDistance;
                                int maxDistance;
                                FindMinMaxDistance(searcherPoint, spatialTreeNode, out minDistance, out maxDistance);
                                if (minDistance <= currentMinDistance && maxDistance <= currentMaxDistance)
                                {
                                    currentMinDistance = minDistance;
                                    currentMaxDistance = maxDistance;
                                }
                                searchList.Add(spatialTreeNode);
                            }
                    }
                }
                if (searchList.Count == 0)
                    throw new ItemNotFound("Item does not exist.");
                startNodeList.Clear();
                foreach (SpatialTreeNode spatialTreeNode in searchList)
                {
                    int minDistance;
                    int maxDistance;
                    FindMinMaxDistance(searcherPoint, spatialTreeNode, out minDistance, out maxDistance);
                    if ((minDistance >= currentMinDistance && minDistance <= currentMaxDistance) ||
                        (maxDistance >= currentMinDistance && maxDistance <= currentMaxDistance) ||
                        (minDistance < currentMinDistance && maxDistance > currentMaxDistance))
                    {
                        startNodeList.Add(spatialTreeNode);
                    }
                }
                level++;
            }
            // Now we have startNodeList filled with all the relevant leaf nodes containing the required item, so pick one and grab the item from it.
            // Atm it just picks the first in the list, needs to be improved.
            return startNodeList.First().GetResource(itemType);
        }

        private static void FindMinMaxDistance(Point searcherPoint, SpatialTreeNode spatialTreeNode, out int minDistance, out int maxDistance)
        {
            // Find the possible minimum and maximum distances for this node as we only want to accrue sections that provide a minium range
            if (spatialTreeNode.TopLeft.X <= searcherPoint.X &&
                spatialTreeNode.BottomRight.X >= searcherPoint.X &&
                spatialTreeNode.TopLeft.Y <= searcherPoint.Y &&
                spatialTreeNode.BottomRight.Y >= searcherPoint.Y)
            {
                minDistance = 0;
                int xDist = Math.Max((searcherPoint.X - spatialTreeNode.TopLeft.X), (spatialTreeNode.BottomRight.X - searcherPoint.X));
                int yDist = Math.Max((searcherPoint.Y - spatialTreeNode.TopLeft.Y), (spatialTreeNode.BottomRight.Y - searcherPoint.Y));
                maxDistance = xDist + yDist;
            }
            else if (spatialTreeNode.TopLeft.X <= searcherPoint.X &&
                spatialTreeNode.BottomRight.X >= searcherPoint.X)
            {
                minDistance = Math.Min(Math.Abs(searcherPoint.Y - spatialTreeNode.TopLeft.Y), Math.Abs(spatialTreeNode.BottomRight.Y - searcherPoint.Y));
                maxDistance = Math.Max(Math.Abs(searcherPoint.Y - spatialTreeNode.TopLeft.Y), Math.Abs(spatialTreeNode.BottomRight.Y - searcherPoint.Y));
            }
            else if (spatialTreeNode.TopLeft.Y <= searcherPoint.Y &&
                spatialTreeNode.BottomRight.Y >= searcherPoint.Y)
            {
                minDistance = Math.Min(Math.Abs(searcherPoint.X - spatialTreeNode.TopLeft.X), Math.Abs(spatialTreeNode.BottomRight.X - searcherPoint.X));
                maxDistance = Math.Max(Math.Abs(searcherPoint.X - spatialTreeNode.TopLeft.X), Math.Abs(spatialTreeNode.BottomRight.X - searcherPoint.X));
            }
            else
            {
                int maxXDist = Math.Max(Math.Abs(searcherPoint.X - spatialTreeNode.TopLeft.X), Math.Abs(spatialTreeNode.BottomRight.X - searcherPoint.X));
                int maxYDist = Math.Max(Math.Abs(searcherPoint.Y - spatialTreeNode.TopLeft.Y), Math.Abs(spatialTreeNode.BottomRight.Y - searcherPoint.Y));
                maxDistance = maxXDist + maxYDist;
                int minXDist = Math.Min(Math.Abs(searcherPoint.X - spatialTreeNode.TopLeft.X), Math.Abs(spatialTreeNode.BottomRight.X - searcherPoint.X));
                int minYDist = Math.Min(Math.Abs(searcherPoint.Y - spatialTreeNode.TopLeft.Y), Math.Abs(spatialTreeNode.BottomRight.Y - searcherPoint.Y));
                minDistance = minXDist + minYDist;
            }
        }
    }
}
