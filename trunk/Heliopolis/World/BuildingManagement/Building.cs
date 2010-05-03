using System;
using System.Collections.Generic;
using System.Linq;
using Heliopolis.Utilities.PathFinder;
using Heliopolis.World.Environment;
using Heliopolis.World.ItemManagement;
using Microsoft.Xna.Framework;

namespace Heliopolis.World.BuildingManagement
{
    /// <summary>
    /// Represents a building in the game world.
    /// </summary>
    [Serializable]
    public class Building : GameWorldObject, ICloneable, ICanHoldItem, ICanAccess
    {
        private BuildingTile[,] _buildingTiles;
        private readonly List<string> _requiredMaterials;
        private List<Item> _constructionItems;
        private Point _position;
        private BuildingStates _buildingState;
        private Point _mainAccessPoint;
        private bool _usesMainAccessPoint;
        private Dictionary<Point, BuildingTileContains> _itemsHeld;

        /// <summary>
        /// Initialises a new instance of the Building class.
        /// </summary>
        /// <param name="size">The size in tiles of the building.</param>
        /// <param name="buildingTiles">A 2D array containing all the building tiles.</param>
        /// <param name="requiredMaterials">The materials required for building.</param>
        /// <param name="owner">The owning game world.</param>
        public Building(Point size, BuildingTile[,] buildingTiles, List<string> requiredMaterials, GameWorld owner) : base (owner)
        {
            Size = size;
            _buildingTiles = buildingTiles;
            _requiredMaterials = requiredMaterials;
            _constructionItems = new List<Item>();
            _buildingState = BuildingStates.None;
        }

        /// <summary>
        /// The size of the building.
        /// </summary>
        public Point Size { get; private set; }

        /// <summary>
        /// The actual tiles that make up this building.
        /// </summary>
        public BuildingTile[,] BuildingTiles
        {
            get { return _buildingTiles; }
            set { _buildingTiles = value; }
        }

        /// <summary>
        /// The required materials.
        /// </summary>
        public List<string> RequiredMaterials
        {
            get { return _requiredMaterials; }
        }

        /// <summary>
        /// The position in the game world of the top left tile of this building.
        /// </summary>
        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// The current state of this building.
        /// </summary>
        public BuildingStates BuildingState
        {
            get { return _buildingState; }
            set { _buildingState = value; }
        }

        /// <summary>
        /// The access point for actors to interact with this building.
        /// </summary>
        public Point MainAccessPoint
        {
            get { return _mainAccessPoint; }
            set { _mainAccessPoint = value; }
        }

        /// <summary>
        /// If this building actually has a main access point.
        /// </summary>
        public bool UsesMainAccessPoint
        {
            get { return _usesMainAccessPoint; }
            set { _usesMainAccessPoint = value; }
        }

        /// <summary>
        /// All the items in this building.
        /// </summary>
        public Dictionary<Point, BuildingTileContains> ItemsHeld
        {
            get { return _itemsHeld; }
            set { _itemsHeld = value; }
        }

        #region ICloneable Members

        /// <summary>
        /// Creates a copy of this building.
        /// </summary>
        /// <returns>A Building copy.</returns>
        public object Clone()
        {
            Building returnMe = (Building) MemberwiseClone();
            returnMe._constructionItems = new List<Item>();
            returnMe._itemsHeld = new Dictionary<Point, BuildingTileContains>();
            _itemsHeld = new Dictionary<Point, BuildingTileContains>();
            foreach (BuildingTile tile in _buildingTiles)
            {
                if (tile.ItemSpace != 0)
                    _itemsHeld.Add(tile.Position, new BuildingTileContains(tile.ItemSpace));
            }
            return returnMe;
        }

        #endregion

        /// <summary>
        /// Starts construction of this building.
        /// </summary>
        public void StartBuilding()
        {
            _buildingState = BuildingStates.UnderConstruction;
            foreach (BuildingTile buildingTile in _buildingTiles)
            {
                EnvironmentTile targetTile = Owner.Environment[_position.X + buildingTile.Position.X, _position.Y + buildingTile.Position.Y];
                targetTile.CanAccess = false;
            }
        }

        /// <summary>
        /// Completes construction of this building.
        /// </summary>
        public void CompleteBuilding()
        {
            _buildingState = BuildingStates.Ready;
            foreach (BuildingTile buildingTile in _buildingTiles)
            {
                EnvironmentTile targetTile = Owner.Environment[_position.X + buildingTile.Position.X, _position.Y + buildingTile.Position.Y];
                targetTile.CanAccess = buildingTile.CanAccess;
                targetTile.BuildingTile = buildingTile;
            }
        }

        /// <summary>
        /// Checks to see if this building can hold a certain item.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="reserveSpace"></param>
        /// <returns></returns>
        public bool CanHold(string itemType, bool reserveSpace)
        {
            foreach (BuildingTileContains container in _itemsHeld.Values.Where(container => container.CanHold(itemType)))
            {
                container.ItemType = itemType;
                container.SpaceReserved++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if this building has a particular item.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="reserveItem"></param>
        /// <returns></returns>
        public bool HasItem(string itemType, bool reserveItem)
        {
            foreach (BuildingTileContains container in _itemsHeld.Values.Where(container => container.HasItem(itemType)))
            {
                if (reserveItem)
                    container.ItemsReserved++;
                return true;
            }
            return false;
        }

        #region ICanHoldItem Members

        /// <summary>
        /// Place an item into this building.
        /// </summary>
        /// <param name="item">The item to place.</param>
        public ItemStates PickupItem(Item item)
        {
            switch (_buildingState)
            {
                case BuildingStates.UnderConstruction:
                    _constructionItems.Add(item);
                    return ItemStates.ConstructionMaterial;
                    break;
                case BuildingStates.Ready:
                    foreach (BuildingTileContains container in _itemsHeld.Values)
                    {
                        if (container.CanHold(item.ItemType))
                        {
                            container.AddItem(item);
                            return ItemStates.InStorage;
                        }
                    }
                    throw new NotSupportedException();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Give an item to another ICanHoldItem.
        /// </summary>
        /// <param name="item">The item to give.</param>
        public void PutdownItem(Item item)
        {
            if (_buildingState == BuildingStates.Ready)
            {
                foreach (BuildingTileContains container in _itemsHeld.Values)
                {
                    //if (container.HasItem(itemType))
                    //{
                    //    Item item = container.RemoveItem();
                    //    container.itemType = item.ItemType;
                    //    container.amount--;
                    //    item.Holder = itemHolder;
                    //    item.ItemState = ItemStates.BeingCarried;
                    //}
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets all the points that provide access from an area.
        /// </summary>
        /// <returns>A MovementDestination object that the accessing object can use to path to this building.</returns>
        public IEnumerable<Point> GetAllAccessPoints()
        {
            List<Point> returnMe = new List<Point>();
            for (int i = 0; i < Size.X; i++)
            {
                Point yPosAbove = new Point(_position.X + i, _position.Y - 1);
                if (yPosAbove.Y >= 0)
                    returnMe.Add(yPosAbove);
                Point yPosBelow = new Point(_position.X + i, _position.Y + Size.Y);
                if (yPosBelow.Y <= Owner.Environment.WorldSize.Y)
                    returnMe.Add(yPosBelow);
            }
            for (int i = 0; i < Size.Y; i++)
            {
                Point xPosLeft = new Point(_position.X - 1, _position.Y + i);
                if (xPosLeft.X >= 0)
                    returnMe.Add(xPosLeft);
                Point xPosRight = new Point(_position.X + Size.X - 1, _position.Y + i);
                if (xPosRight.X <= Owner.Environment.WorldSize.X)
                    returnMe.Add(xPosRight);
            }
            return returnMe;
        }
    }
}
