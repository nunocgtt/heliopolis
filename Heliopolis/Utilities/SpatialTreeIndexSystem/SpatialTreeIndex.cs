using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Heliopolis.World;

namespace Heliopolis.Utilities.SpatialTreeIndexSystem
{
    /// <summary>
    /// Enumerates the various type of objects held in the SpatialTreeIndex.
    /// </summary>
    public enum SpatialObjectType
    {
        Item = 0,
        Actor = 1,
        Storage = 2,
        EnvironmentTile = 3
    }

    /// <summary>
    /// Represents a spatial index for fast retrieval of objects close to a point. Also manages object
    /// separation into "sections".
    /// </summary>
    [Serializable]
    public class SpatialTreeIndex 
    {
        public SpatialTreeIndex(Point sectionSize, Point worldSize, int [] treeWidth)
        {
            SectionSize = sectionSize;
            WorldSize = worldSize;
            TreeWidth = treeWidth;
        }

        private Point SectionSize { get; set; }
        private Point WorldSize { get; set; }

        /// <summary>
        /// This number squared will give the number of branches at level
        /// </summary>
        public int[] TreeWidth { get; private set; }
        /// <summary>
        /// A grid which provides quick access to an Environment Tile's relevant spatial tree node.
        /// </summary>
        public SpatialTreeNode[,] GridToNodes;

        private Dictionary<int, Dictionary<Point, List<ISpatialIndexMember>>> _objectsBySection;
        
        public static int PointToIndex(Point convertMe, int xandysize)
        {
            return convertMe.X + convertMe.Y * xandysize;
        }

        public static Point IndexToPoint(int convertMe, int xandysize)
        {
            throw new Exception("Not implemented.");
        }

        private Point PositionToSection(Point position)
        {
            return new Point((position.X / SectionSize.X), (position.Y / SectionSize.Y));
        }

        public void CheckChangeSection(Point oldPosition, Point newPosition, ISpatialIndexMember objectToMove, SpatialObjectType spatialObjectType, string resourceType)
        {
            if (oldPosition == new Point(-1, -1)) return;
            Point oldSection = PositionToSection(oldPosition);
            Point newSection = PositionToSection(newPosition);
            if (oldSection != newSection)
            {
                Dictionary<Point, List<ISpatialIndexMember>> objectTypeBySection = _objectsBySection[(int)spatialObjectType];
                if (spatialObjectType == SpatialObjectType.Item)
                    GridToNodes[oldPosition.X, oldPosition.Y].ChangeResourceCount(resourceType, -1);
                objectTypeBySection[oldSection].Remove(objectToMove);
                objectTypeBySection[newSection].Add(objectToMove);
                if (spatialObjectType == SpatialObjectType.Item)
                    GridToNodes[newPosition.X, newPosition.Y].ChangeResourceCount(resourceType, 1);
            }
        }

        public void AddToSection(Point position, ISpatialIndexMember objectToAdd, SpatialObjectType spatialObjectType, string resourceType)
        {
            Point sectionToAdd = PositionToSection(position);
            Dictionary<Point, List<ISpatialIndexMember>> objectTypeBySection = _objectsBySection[(int)spatialObjectType];
            objectTypeBySection[sectionToAdd].Add(objectToAdd);
            //Items are tracked in the tree
            if (spatialObjectType == SpatialObjectType.Item)
                GridToNodes[position.X, position.Y].ChangeResourceCount(resourceType, 1);
        }

        public void RemoveFromSection(Point position, ISpatialIndexMember objectToRemove, SpatialObjectType spatialObjectType, string resourceType)
        {
            Point sectionToRemoveFrom = PositionToSection(position);
            Dictionary<Point, List<ISpatialIndexMember>> objectTypeBySection = _objectsBySection[(int)spatialObjectType];
            if (spatialObjectType == SpatialObjectType.Item)
                GridToNodes[position.X, position.Y].ChangeResourceCount(resourceType, -1);
            objectTypeBySection[sectionToRemoveFrom].Remove(objectToRemove);
        }

        public List<ISpatialIndexMember> SectionToRender(Point section, SpatialObjectType spatialObjectType)
        {
            return _objectsBySection[(int)spatialObjectType][section];
        }

        public void Initialise()
        {
            _topNode = new SpatialTreeNode(new Point(0, 0), new Point(WorldSize.X - 1, WorldSize.Y - 1), 0, TreeWidth.Length - 1, null, this);
            GridToNodes = new SpatialTreeNode[WorldSize.X, WorldSize.Y];
            _objectsBySection = new Dictionary<int, Dictionary<Point, List<ISpatialIndexMember>>>();
            foreach (int i in Enum.GetValues(typeof(SpatialObjectType)))
            {
                Point topLeftSecion = new Point(0, 0);
                Point bottomRightSection = PositionToSection(new Point(WorldSize.X, WorldSize.Y));
                Dictionary<Point, List<ISpatialIndexMember>> addMe = new Dictionary<Point, List<ISpatialIndexMember>>();
                _objectsBySection.Add(i, addMe);
                for (int x = topLeftSecion.X; x <= bottomRightSection.X; x++)
                    for (int y = topLeftSecion.Y; y <= bottomRightSection.Y; y++)
                    {
                        addMe.Add(new Point(x, y), new List<ISpatialIndexMember>());
                    }
            }
            _topNode.Construct();
        }

        private SpatialTreeNode _topNode;

        public SpatialTreeNode TopNode
        {
            get { return _topNode; }
            set { _topNode = value; }
        }


        public ISpatialIndexMember FindClosestObject(Point searcherPoint, string objectType)
        {
            if (_topNode.ResourceCount.ContainsKey(objectType))
            {
                if (_topNode.ResourceCount[objectType] > 0)
                {
                    return SearchTree(searcherPoint, objectType);
                }
                else
                    throw new ItemNotFound("No item available");
            }
            else
                throw new ItemNotFound("No item available");
        }

        private ISpatialIndexMember SearchTree(Point searcherPoint, string itemType)
        {
            List<SpatialTreeNode> startNodeList = new List<SpatialTreeNode>();
            startNodeList.Add(_topNode);

            int level = _topNode.Level;
            int maxLevel = _topNode.MaxLevel;

            while (level < maxLevel)
            {
                int currentMinDistance = Int32.MaxValue;
                int currentMaxDistance = Int32.MaxValue;
                List<SpatialTreeNode> searchList = new List<SpatialTreeNode>();
                foreach (SpatialTreeNode startSpatialTreeNode in startNodeList)
                {
                    foreach (SpatialTreeNode spatialTreeNode in startSpatialTreeNode.Children.Values)
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
            return _objectsBySection[(int)SpatialObjectType.Item][PositionToSection(startNodeList[0].TopLeft)][0];
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
