using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Heliopolis.Utilities;

namespace Heliopolis.World
{
    /// <summary>
    /// Represents a game environment.
    /// </summary>
    /// <remarks>This class is like an EnvironmentTile manager. It contains all the tiles in the game world
    /// but manages a lot of metadata for tiles. The reason why there is so much metadata is because this game
    /// environment has to be searched and interrogated using efficient algorithms.
    /// The PathFinder class will use the Environment to perform its search.</remarks>
    [Serializable]
    public class Environment : GameWorldObject, ISearchAble<Point>
    {
        // keeping the pathing array different because eventually
        // i want to multithread the pathing routine and this will require locking/unlocking
        private EnvironmentTile[,] gameWorld;
        private Point worldSize;
        private Dictionary<int, Area> areaDictionary;
        [NonSerialized]
        private EdgeTraverse edgeTraversal;
        // Constants
        private int maxPathNodes = 1000;

        /// <summary>
        /// The tile at position (X,Y)
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <returns>The tile.</returns>
        public EnvironmentTile this[int x, int y]
        {
            get { return gameWorld[x, y]; }
        }

        /// <summary>
        /// The tile at a particular Point.
        /// </summary>
        /// <param name="pos">The position of the tile.</param>
        /// <returns>The tile.</returns>
        public EnvironmentTile this[Point pos]
        {
            get { return gameWorld[pos.X, pos.Y]; }
        }

        /// <summary>
        /// All the tiles per area.
        /// </summary>
        public Dictionary<int, Area> AreaDictionary
        {
            get { return areaDictionary; }
            set { areaDictionary = value; }
        }

        /// <summary>
        /// Maintains edge metadata. Used for efficient joining and separating of areas.
        /// </summary>
        public EdgeTraverse EdgeTraversal
        {
            get { return edgeTraversal; }
            set { edgeTraversal = value; }
        }

        /// <summary>
        /// The size of the world.
        /// </summary>
        public Point WorldSize
        {
            get { return worldSize; }
            set { worldSize = value; }
        }

        /// <summary>
        /// All the tiles in this environment.
        /// </summary>
        public EnvironmentTile[,] GameWorld
        {
            get { return gameWorld; }
            set
            {
                gameWorld = value;
                setTileLinks();
            }
        }

        public List<Point> GetSuccessors(Point point)
        {
            return gameWorld[point.X, point.Y].GetAccessPoints();
        }

        public List<Node<Point>> GetSuccessors(Point point, Point parentPoint)
        {
            bool hasParent = (parentPoint == null);
            List<Node<Point>> returnList = new List<Node<Point>>();

            EnvironmentTile currentTile = gameWorld[point.X, point.Y];
            EnvironmentTile parentTile = hasParent ? gameWorld[parentPoint.X, parentPoint.Y] : null;

            if (currentTile.LeftTile != null)
            {
                if (currentTile.LeftTile.CanAccess)
                {
                    //Note: Those directions are where the node is coming *FROM*
                    Node<Point> NewNode = new Node<Point>(currentTile.LeftTile.Position, Direction.East);
                    returnList.Add(NewNode);
                }
            }
            if (currentTile.RightTile != null)
            {
                if (currentTile.RightTile.CanAccess)
                {
                    Node<Point> NewNode = new Node<Point>(currentTile.RightTile.Position, Direction.West);
                    returnList.Add(NewNode);
                }
            }
            if
                (currentTile.TopTile != null)
            {
                if (currentTile.TopTile.CanAccess)
                {
                    Node<Point> NewNode = new Node<Point>(currentTile.TopTile.Position, Direction.South);
                    returnList.Add(NewNode);
                }
            }

            if (currentTile.BottomTile != null)
            {
                if (currentTile.BottomTile.CanAccess)
                {
                    Node<Point> NewNode = new Node<Point>(currentTile.BottomTile.Position, Direction.North);
                    returnList.Add(NewNode);
                }
            }
            // Remove the parent if we have one
            if (hasParent)
            {
                foreach (Node<Point> n in returnList)
                {
                    if (n.Position == parentTile.Position)
                    {
                        returnList.Remove(n);
                        break;
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// The movement weighting of a tile.
        /// </summary>
        /// <param name="point">The position of the tile.</param>
        /// <returns>A float representing the weight.</returns>
        public float getPathingWeight(Point point)
        {
            // At the moment all the grid tiles have the same pathing weight
            return 1f;
        }

        /// <summary>
        /// Initialises a new instance of the Environment class.
        /// </summary>
        /// <param name="_worldSize">The size of the game world.</param>
        /// <param name="_owner">The owning game world.</param>
        public Environment(Point _worldSize, GameWorld _owner) : base(_owner)
        {
            worldSize = _worldSize;
            gameWorld = new EnvironmentTile[worldSize.X, worldSize.Y];
            areaDictionary = new Dictionary<int, Area>();
        }

        public void InitialiseEnvironment()
        {
            Global.PathFinder.Initialise(maxPathNodes, this, worldSize);
            Global.FillFinder.Initialise(this);
            edgeTraversal = new EdgeTraverse(this);
        }

        /// <summary>
        /// Sets all the tile pointers to their adjacent tiles.
        /// </summary>
        private void setTileLinks()
        {
            for (int i = 0; i < worldSize.X; i++)
            {
                for (int j = 0; j < worldSize.Y; j++)
                {
                    if (i != 0)
                        gameWorld[i, j].LeftTile = gameWorld[i - 1, j];
                    if (i != worldSize.X - 1)
                        gameWorld[i, j].RightTile = gameWorld[i + 1, j];
                    if (j != 0)
                        gameWorld[i, j].TopTile = gameWorld[i, j - 1];
                    if (j != worldSize.Y - 1)
                        gameWorld[i, j].BottomTile = gameWorld[i, j + 1];
                }
            }
        }

        /// <summary>
        /// Create a tile.
        /// </summary>
        /// <param name="type">The tile to create.</param>
        /// <param name="position">The position of the tile.</param>
        private void spawnTile(string type, Point position)
        {
            gameWorld[position.X, position.Y] = EnvironmentTileFactory.GetNewTile(type, position);
            owner.SpatialTreeIndex.AddToSection(position, gameWorld[position.X, position.Y], SpatialObjectType.EnvironmentTile, "");
        }

        /// <summary>
        /// Loads a test environment.
        /// </summary>
        public void LoadTestEnvironment()
        {
            owner.SpatialTreeIndex.Initialise();
            Random someNumber = new Random(0);
            for (int i = 0; i < worldSize.X; i++)
            {
                for (int j = 0; j < worldSize.Y; j++)
                {
                    int tileToUse = someNumber.Next(0, 6);
                    if (tileToUse > 3)
                    {
                        spawnTile("grass", new Point(i, j));
                    }
                    else
                    {
                        spawnTile("stone", new Point(i, j));
                    }
                }
            }
            InitialiseHelperClasses();
        }

        public void InitialiseHelperClasses()
        {
            setTileLinks();
            edgeTraversal.buildEdgeData();
            BuildInitialWallGroup();
            BuildInitialGroups();
        }

        /// <summary>
        /// Find the first tile where the Area ID is Zero (ie not in a group).
        /// </summary>
        /// <returns></returns>
        public EnvironmentTile FindUngroupedTile()
        {
            for (int i = 0; i < worldSize.X; i++)
            {
                for (int j = 0; j < worldSize.Y; j++)
                {
                    if (gameWorld[i, j].AreaID == 0)
                    {
                        return gameWorld[i, j];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Puts all the unacessable tiles into the group of ID -1.
        /// </summary>
        public void BuildInitialWallGroup()
        {
            if (!areaDictionary.ContainsKey(-1))
            {
                areaDictionary.Add(-1, new Area(-1));
            }
            for (int i = 0; i < worldSize.X; i++)
            {
                for (int j = 0; j < worldSize.Y; j++)
                {
                    if (!this[i,j].CanAccess)
                    {
                        areaDictionary[-1].memberCount++;
                        areaDictionary[-1].members.Add(this[i, j]);
                    }
                }
            }
        }

        /// <summary>
        /// Builds all the areas.
        /// </summary>
        public void BuildInitialGroups()
        {
            int nextGroupId = 0;
            EnvironmentTile startTile;
            while ((startTile = FindUngroupedTile()) != null)
            {
                nextGroupId++;
                if (!areaDictionary.ContainsKey(nextGroupId))
                {
                    areaDictionary.Add(nextGroupId, new Area(nextGroupId));
                }
                FillRequest<Point> newRequest = new FillRequest<Point>(startTile.Position);
                Global.FillFinder.NewSearch(newRequest);
                SearchState returnState = Global.FillFinder.SearchStep(worldSize.X * worldSize.Y);
                if (returnState == SearchState.SEARCH_STATE_SUCCEEDED)
                {
                    FillfindAnswer<Point> theAnswer = Global.FillFinder.finalResult();
                    foreach (Point p in theAnswer.pointsFilled)
                    {
                        gameWorld[p.X, p.Y].AreaID = nextGroupId;
                        areaDictionary[nextGroupId].memberCount++;
                        areaDictionary[nextGroupId].members.Add(gameWorld[p.X, p.Y]);
                    }
                }
            }
        }

        /// <summary>
        /// If a tile access flag has changed, this could preclude the joining or separation
        /// of areas. This method will handle that logic of joining/separating.
        /// </summary>
        /// <param name="theTile">The tile changing state.</param>
        public void ManageAccessStateChange(EnvironmentTile theTile)
        {
            edgeTraversal.CanAccessChanged(theTile);
            if (theTile.CanAccess)
            {
                areaDictionary[-1].members.Remove(theTile);
                areaDictionary[-1].memberCount--;
                // Here we need to check if two areas have merged
                List<Point> accessPoints = theTile.GetAccessPoints();
                foreach (Point point in accessPoints)
                {
                    foreach (Point pointTwo in accessPoints)
                    {
                        if (point != pointTwo)
                        {
                            EnvironmentTile firstTile = this[point];
                            EnvironmentTile secondTile = this[pointTwo];
                            if (firstTile.AreaID != secondTile.AreaID)
                            {
                                // Merge requried
                                Area mergeOne = areaDictionary[firstTile.AreaID];
                                Area mergeTwo = areaDictionary[secondTile.AreaID];
                                theTile.AreaID = Area.MergeTwoAreas(mergeOne, mergeTwo);
                            }                                
                        }
                    }
                }
                if (theTile.AreaID == -1)
                {
                    theTile.AreaID = this[accessPoints[0]].AreaID;
                }
            }
            else
            {
                // now here we need to use the edge state to determine splitting up an area
                areaDictionary[theTile.AreaID].members.Remove(theTile);
                areaDictionary[theTile.AreaID].memberCount--;
                theTile.AreaID = -1;
            }
            areaDictionary[theTile.AreaID].members.Add(theTile);
            areaDictionary[theTile.AreaID].memberCount++;
        }


    }


}
