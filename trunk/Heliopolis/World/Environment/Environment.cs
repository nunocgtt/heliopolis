using System;
using System.Collections.Generic;
using System.Linq;
using Heliopolis.GraphicsEngine;
using Heliopolis.Utilities.PathFinder;
using Heliopolis.Utilities.SpatialTreeIndexSystem;
using Microsoft.Xna.Framework;
using Heliopolis.Utilities;

namespace Heliopolis.World.Environment
{
    /// <summary>
    /// Represents a game environment.
    /// </summary>
    /// <remarks>This class is like an EnvironmentTile manager. It contains all the tiles in the game world
    /// but manages a lot of metadata for tiles. The reason why there is so much metadata is because this game
    /// environment has to be searched and interrogated using efficient algorithms.
    /// The PathFinder class will use the Environment to perform its search.</remarks>
    [Serializable]
    public class Environment : GameWorldObject, ISearchAble<Point>, IIsometricTileProvider, IWorld
    {
        // keeping the pathing array different because eventually
        // i want to multithread the pathing routine and this will require locking/unlocking
        private EnvironmentTile[,] _gameWorld;
        private Point _worldSize;
        private Dictionary<int, Area> _areaDictionary;
        [NonSerialized]
        private EdgeTraverse _edgeTraversal;
        // Constants
        private const int MaxPathNodes = 1000;

        /// <summary>
        /// The tile at position (X,Y)
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <returns>The tile.</returns>
        public EnvironmentTile this[int x, int y]
        {
            get { return _gameWorld[x, y]; }
        }

        /// <summary>
        /// The tile at a particular Point.
        /// </summary>
        /// <param name="pos">The position of the tile.</param>
        /// <returns>The tile.</returns>
        public EnvironmentTile this[Point pos]
        {
            get { return _gameWorld[pos.X, pos.Y]; }
        }

        /// <summary>
        /// All the tiles per area.
        /// </summary>
        public Dictionary<int, Area> AreaDictionary
        {
            get { return _areaDictionary; }
            set { _areaDictionary = value; }
        }

        /// <summary>
        /// Maintains edge metadata. Used for efficient joining and separating of areas.
        /// </summary>
        public EdgeTraverse EdgeTraversal
        {
            get { return _edgeTraversal; }
            set { _edgeTraversal = value; }
        }

        /// <summary>
        /// The size of the world.
        /// </summary>
        public Point WorldSize
        {
            get { return _worldSize; }
            set { _worldSize = value; }
        }

        /// <summary>
        /// All the tiles in this environment.
        /// </summary>
        public EnvironmentTile[,] GameWorld
        {
            get { return _gameWorld; }
            set
            {
                _gameWorld = value;
                SetTileLinks();
            }
        }

        public List<Point> GetSuccessors(Point point)
        {
            return _gameWorld[point.X, point.Y].GetAccessPoints();
        }

        public List<Node<Point>> GetSuccessorsWithDir(Point point, Point parentPoint)
        {
            bool hasParent = (parentPoint == null);
            List<Node<Point>> returnList = new List<Node<Point>>();

            EnvironmentTile currentTile = _gameWorld[point.X, point.Y];
            EnvironmentTile parentTile = hasParent ? _gameWorld[parentPoint.X, parentPoint.Y] : null;

            if (currentTile.WestTile != null)
            {
                if (currentTile.WestTile.CanAccess)
                {
                    //Note: Those directions are where the node is coming *FROM*
                    Node<Point> newNode = new Node<Point>(currentTile.WestTile.Position, Direction.East);
                    returnList.Add(newNode);
                }
            }
            if (currentTile.EastTile != null)
            {
                if (currentTile.EastTile.CanAccess)
                {
                    Node<Point> newNode = new Node<Point>(currentTile.EastTile.Position, Direction.West);
                    returnList.Add(newNode);
                }
            }
            if (currentTile.NorthTile != null)
            {
                if (currentTile.NorthTile.CanAccess)
                {
                    Node<Point> newNode = new Node<Point>(currentTile.NorthTile.Position, Direction.South);
                    returnList.Add(newNode);
                }
            }
            if (currentTile.SouthTile != null)
            {
                if (currentTile.SouthTile.CanAccess)
                {
                    Node<Point> newNode = new Node<Point>(currentTile.SouthTile.Position, Direction.North);
                    returnList.Add(newNode);
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
        public float GetPathingWeight(Point point)
        {
            // At the moment all the grid tiles have the same pathing weight
            return 1f;
        }

        /// <summary>
        /// Initialises a new instance of the Environment class.
        /// </summary>
        /// <param name="worldSize">The size of the game world.</param>
        /// <param name="owner">The owning game world.</param>
        public Environment(Point worldSize, GameWorld owner) : base(owner)
        {
            this._worldSize = worldSize;
            _gameWorld = new EnvironmentTile[this._worldSize.X, this._worldSize.Y];
            _areaDictionary = new Dictionary<int, Area>();
        }

        public void InitialiseEnvironment()
        {
            Global.PathFinder.Initialise(MaxPathNodes, this, _worldSize);
            Global.FillFinder.Initialise(this);
            _edgeTraversal = new EdgeTraverse(this);
        }

        /// <summary>
        /// Sets all the tile pointers to their adjacent tiles.
        /// </summary>
        private void SetTileLinks()
        {
            for (int i = 0; i < _worldSize.X; i++)
            {
                for (int j = 0; j < _worldSize.Y; j++)
                {
                    if (i != 0)
                        _gameWorld[i, j].WestTile = _gameWorld[i - 1, j];
                    if (i != _worldSize.X - 1)
                        _gameWorld[i, j].EastTile = _gameWorld[i + 1, j];
                    if (j != 0)
                        _gameWorld[i, j].NorthTile = _gameWorld[i, j - 1];
                    if (j != _worldSize.Y - 1)
                        _gameWorld[i, j].SouthTile = _gameWorld[i, j + 1];
                }
            }
        }

        /// <summary>
        /// Create a tile.
        /// </summary>
        /// <param name="type">The tile to create.</param>
        /// <param name="position">The position of the tile.</param>
        private EnvironmentTile SpawnTile(string type, Point position)
        {
            _gameWorld[position.X, position.Y] = EnvironmentTileFactory.GetNewTile(type, position);
            Owner.SpatialTreeIndex.AddToSection(position, _gameWorld[position.X, position.Y], SpatialObjectType.EnvironmentTile, "");
            return _gameWorld[position.X, position.Y];
        }

        /// <summary>
        /// Loads a test environment.
        /// </summary>
        public void LoadTestEnvironment()
        {
            Owner.SpatialTreeIndex.Initialise();
            Random someNumber = new Random(0);
            for (int i = 0; i < _worldSize.X; i++)
            {
                for (int j = 0; j < _worldSize.Y; j++)
                {
                    int tileToUse = someNumber.Next(0, 6);
                    if (tileToUse >= 5)
                    {
                        EnvironmentTile tileAdded = SpawnTile("grass", new Point(i, j));
                        tileAdded.InteractableObject = new HarvestableInteractableObject(Owner,tileAdded, "tree1", 30, "wood", "Woodchopping");
                    }
                    else if (tileToUse >= 1)
                    {
                        SpawnTile("grass", new Point(i, j));
                    }
                    else
                    {
                        SpawnTile("rock", new Point(i, j));
                    }
                }
            }
            InitialiseHelperClasses();
        }

        public void InitialiseHelperClasses()
        {
            SetTileLinks();
            _edgeTraversal.buildEdgeData();
            BuildInitialWallGroup();
            BuildInitialGroups();
        }

        /// <summary>
        /// Find the first tile where the Area ID is Zero (ie not in a group).
        /// </summary>
        /// <returns></returns>
        public EnvironmentTile FindUngroupedTile()
        {
            for (int i = 0; i < _worldSize.X; i++)
            {
                for (int j = 0; j < _worldSize.Y; j++)
                {
                    if (_gameWorld[i, j].AreaID == 0)
                    {
                        return _gameWorld[i, j];
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
            if (!_areaDictionary.ContainsKey(-1))
            {
                _areaDictionary.Add(-1, new Area(-1));
            }
            for (int i = 0; i < _worldSize.X; i++)
            {
                for (int j = 0; j < _worldSize.Y; j++)
                {
                    if (!this[i,j].CanAccess)
                    {
                        _areaDictionary[-1].MemberCount++;
                        _areaDictionary[-1].Members.Add(this[i, j]);
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
                if (!_areaDictionary.ContainsKey(nextGroupId))
                {
                    _areaDictionary.Add(nextGroupId, new Area(nextGroupId));
                }
                FillRequest<Point> newRequest = new FillRequest<Point>(startTile.Position);
                Global.FillFinder.NewSearch(newRequest);
                SearchState returnState = Global.FillFinder.SearchStep(_worldSize.X * _worldSize.Y);
                if (returnState == SearchState.SearchStateSucceeded)
                {
                    FillfindAnswer<Point> theAnswer = Global.FillFinder.FinalResult();
                    foreach (Point p in theAnswer.PointsFilled)
                    {
                        _gameWorld[p.X, p.Y].AreaID = nextGroupId;
                        _areaDictionary[nextGroupId].MemberCount++;
                        _areaDictionary[nextGroupId].Members.Add(_gameWorld[p.X, p.Y]);
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
            _edgeTraversal.CanAccessChanged(theTile);
            if (theTile.CanAccess)
            {
                _areaDictionary[-1].Members.Remove(theTile);
                _areaDictionary[-1].MemberCount--;
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
                                Area mergeOne = _areaDictionary[firstTile.AreaID];
                                Area mergeTwo = _areaDictionary[secondTile.AreaID];
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
                _areaDictionary[theTile.AreaID].Members.Remove(theTile);
                _areaDictionary[theTile.AreaID].MemberCount--;
                theTile.AreaID = -1;
            }
            _areaDictionary[theTile.AreaID].Members.Add(theTile);
            _areaDictionary[theTile.AreaID].MemberCount++;
        }

        #region IIsometricTileProvider Members

        public List<string> GetTexturesToDraw(Point position)
        {
            List<string> textures = new List<string> {this[position].Texture};
            if (this[position].InteractableObject != null)
            {
                textures.Add(this[position].InteractableObject.Texture);
            }
            textures.AddRange(this[position].ActorsOnTile.Select(drawActor => drawActor.Texture));
            textures.AddRange(this[position].ItemsOnGround.Select(drawItem => drawItem.Texture));
            return textures;
        }

        #endregion

        #region IWorld Members

        Point IWorld.WorldSize
        {
            get
            {
                return _worldSize;
            }
        }

        #endregion
    }


}
