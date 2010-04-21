using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Heliopolis.World;

namespace Heliopolis.Utilities
{
    /// <summary>
    /// Enumerates the various type of objects held in the SpatialTreeIndex.
    /// </summary>
    public enum SpatialObjectType : int
    {
        Item = 0,
        Actor = 1,
        Building = 2,
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
            this.SectionSize = sectionSize;
            this.WorldSize = worldSize;
            this.TreeWidth = treeWidth;
        }

        public Point SectionSize { get; set; }
        public Point WorldSize { get; set; }

        /// <summary>
        /// This number squared will give the number of branches at level
        /// </summary>
        public int[] TreeWidth { get; set; }
        /// <summary>
        /// A grid which provides quick access to an Environment Tile's relevant spatial tree node.
        /// </summary>
        public SpatialTreeNode[,] GridToNodes;

        private Dictionary<int, Dictionary<Point, List<ISpatialIndexMember>>> objectsBySection;
        
        public static int PointToIndex(Point convertMe, int xandysize)
        {
            return convertMe.X + convertMe.Y * xandysize;
        }

        public static Point IndexToPoint(int convertMe, int xandysize)
        {
            throw new Exception("Not implemented.");
        }

        private Point positionToSection(Point position)
        {
            return new Point((position.X / SectionSize.X), (position.Y / SectionSize.Y));
        }

        public void CheckChangeSection(Point oldPosition, Point newPosition, ISpatialIndexMember objectToMove, SpatialObjectType spatialObjectType, string resourceType)
        {
            if (oldPosition != new Point(-1, -1))
            {
                Point oldSection = positionToSection(oldPosition);
                Point newSection = positionToSection(newPosition);
                if (oldSection != newSection)
                {
                    Dictionary<Point, List<ISpatialIndexMember>> objectTypeBySection = objectsBySection[(int)spatialObjectType];
                    if (spatialObjectType == SpatialObjectType.Item)
                        GridToNodes[oldPosition.X, oldPosition.Y].ChangeResourceCount(resourceType, -1);
                    objectTypeBySection[oldSection].Remove(objectToMove);
                    objectTypeBySection[newSection].Add(objectToMove);
                    if (spatialObjectType == SpatialObjectType.Item)
                        GridToNodes[newPosition.X, newPosition.Y].ChangeResourceCount(resourceType, 1);
                }
            }
        }

        public void AddToSection(Point position, ISpatialIndexMember objectToAdd, SpatialObjectType spatialObjectType, string resourceType)
        {
            Point sectionToAdd = positionToSection(position);
            Dictionary<Point, List<ISpatialIndexMember>> objectTypeBySection = objectsBySection[(int)spatialObjectType];
            objectTypeBySection[sectionToAdd].Add(objectToAdd);
            //Items are tracked in the tree
            if (spatialObjectType == SpatialObjectType.Item)
                GridToNodes[position.X, position.Y].ChangeResourceCount(resourceType, 1);
        }

        public void RemoveFromSection(Point position, ISpatialIndexMember objectToRemove, SpatialObjectType spatialObjectType, string resourceType)
        {
            Point sectionToRemoveFrom = positionToSection(position);
            Dictionary<Point, List<ISpatialIndexMember>> objectTypeBySection = objectsBySection[(int)spatialObjectType];
            if (spatialObjectType == SpatialObjectType.Item)
                GridToNodes[position.X, position.Y].ChangeResourceCount(resourceType, -1);
            objectTypeBySection[sectionToRemoveFrom].Remove(objectToRemove);
        }

        public List<ISpatialIndexMember> SectionToRender(Point section, SpatialObjectType spatialObjectType)
        {
            return objectsBySection[(int)spatialObjectType][section];
        }

        public void Initialise()
        {
            topNode = new SpatialTreeNode(new Point(0, 0), new Point(WorldSize.X - 1, WorldSize.Y - 1), 0, TreeWidth.Length - 1, null, this);
            GridToNodes = new SpatialTreeNode[WorldSize.X, WorldSize.Y];
            objectsBySection = new Dictionary<int, Dictionary<Point, List<ISpatialIndexMember>>>();
            foreach (int i in Enum.GetValues(typeof(SpatialObjectType)))
            {
                Point topLeftSecion = new Point(0, 0);
                Point bottomRightSection = positionToSection(new Point(WorldSize.X, WorldSize.Y));
                Dictionary<Point, List<ISpatialIndexMember>> addMe = new Dictionary<Point, List<ISpatialIndexMember>>();
                objectsBySection.Add(i, addMe);
                for (int x = topLeftSecion.X; x <= bottomRightSection.X; x++)
                    for (int y = topLeftSecion.Y; y <= bottomRightSection.Y; y++)
                    {
                        addMe.Add(new Point(x, y), new List<ISpatialIndexMember>());
                    }
            }
            topNode.Construct();
        }

        private SpatialTreeNode topNode;

        public SpatialTreeNode TopNode
        {
            get { return topNode; }
            set { topNode = value; }
        }


        public ISpatialIndexMember FindClosestObject(Point searcherPoint, string objectType)
        {
            if (topNode.ResourceCount.ContainsKey(objectType))
            {
                if (topNode.ResourceCount[objectType] > 0)
                {
                    return searchTree(searcherPoint, objectType);
                }
                else
                    throw new ItemNotFound("No item available");
            }
            else
                throw new ItemNotFound("No item available");
        }

        private ISpatialIndexMember searchTree(Point searcherPoint, string itemType)
        {
            List<SpatialTreeNode> startNodeList = new List<SpatialTreeNode>();
            startNodeList.Add(topNode);

            int level = topNode.Level;
            int maxLevel = topNode.MaxLevel;

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
            return objectsBySection[(int)SpatialObjectType.Item][positionToSection(startNodeList[0].TopLeft)][0];
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
                ResourceCount.Add(resourceName,0);
            }
            ResourceCount[resourceName] += difference;
            if (parent != null)
                parent.ChangeResourceCount(resourceName, difference);
        }

        public void Construct()
        {
            int treeWidth = spatialTreeIndex.TreeWidth[level];
            int newWidth = ((bottomRight.X - topLeft.X)+1) / treeWidth;
            int newHeight = ((bottomRight.Y - topLeft.Y)+1) / treeWidth;
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
