using System;
using System.Collections.Generic;
using System.Linq;
using Heliopolis.Utilities.PathFinder;
using Heliopolis.World.Environment;
using Heliopolis.World.ItemManagement;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{
    /// <summary>
    /// The different states a building can be in.
    /// </summary>
    public enum BuildingStates
    {
        /// <summary>
        /// Currently getting constructed.
        /// </summary>
        UnderConstruction,
        /// <summary>
        /// Ready and constructed.
        /// </summary>
        Ready,
        /// <summary>
        /// In no state at the moment.
        /// </summary>
        None
    }

    /// <summary>
    /// Represents a building in the game world.
    /// </summary>
    [Serializable]
    public class Building : GameWorldObject, ICloneable, ICanHoldItem, ICanAccess
    {
        private BuildingTile[,] _buildingTiles;
        private readonly List<string> _requiredMaterials;
        private readonly List<int> _requiredMaterialAmount;
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
        /// <param name="requiredMaterialAmounts">The quantities of the materials.</param>
        /// <param name="owner">The owning game world.</param>
        public Building(Point size, BuildingTile[,] buildingTiles, List<string> requiredMaterials, List<int> requiredMaterialAmounts, GameWorld owner) : base (owner)
        {
            Size = size;
            _buildingTiles = buildingTiles;
            _requiredMaterials = requiredMaterials;
            _requiredMaterialAmount = requiredMaterialAmounts;
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
        /// The required material quantites.
        /// </summary>
        public List<int> RequiredMaterialAmount
        {
            get { return _requiredMaterialAmount; }
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
                container.itemType = itemType;
                container.spaceReserved++;
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
                    container.itemsReserved++;
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
                    item.Holder = this;
                    item.ItemState = ItemStates.BeingCarried;
                    break;
                case BuildingStates.Ready:
                    foreach (BuildingTileContains container in _itemsHeld.Values)
                    {
                        if (container.CanHold(item.ItemType))
                        {
                            container.AddItem(item);
                            item.Holder = this;
                            item.ItemState = ItemStates.BeingCarried;
                        }
                    }
                    break;
            }
            return ItemStates.InStorage;
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
        /// The points of access that actors can get to, for constrution of this building.
        /// </summary>
        /// <param name="areaId">The area that the accessing actor is in.</param>
        /// <returns>A List of Point where the actor can go to access this building.</returns>
        public List<Point> ConstructionPoints(int areaId)
        {
            List<Point> returnMe = new List<Point>();
            for (int i = 0; i < Size.X; i++)
            {
                Point yPosAbove = new Point(_position.X + i,_position.Y - 1);
                if (yPosAbove.Y >= 0)
                    if (Owner.Environment[yPosAbove].CanAccess && Owner.Environment[yPosAbove].AreaID == areaId)
                        returnMe.Add(yPosAbove);
                Point yPosBelow = new Point(_position.X + i, _position.Y + Size.Y);
                if (yPosBelow.Y <= Owner.Environment.WorldSize.Y)
                    if (Owner.Environment[yPosBelow].CanAccess && Owner.Environment[yPosBelow].AreaID == areaId)
                        returnMe.Add(yPosBelow);
            }
            for (int i = 0; i < Size.Y; i++)
            {
                Point xPosLeft = new Point(_position.X - 1, _position.Y + i);
                if (xPosLeft.X >= 0)
                    if (Owner.Environment[xPosLeft].CanAccess && Owner.Environment[xPosLeft].AreaID == areaId)
                        returnMe.Add(xPosLeft);
                Point xPosRight = new Point(_position.X + Size.X-1, _position.Y + i);
                if (xPosRight.X <= Owner.Environment.WorldSize.X)
                    if (Owner.Environment[xPosRight].CanAccess && Owner.Environment[xPosRight].AreaID == areaId)
                        returnMe.Add(xPosRight);
            }
            return returnMe;
        }

        /// <summary>
        /// Checks to see if this building can be access from a certain area.
        /// </summary>
        /// <param name="areaId">The ID of the area of the object wanting access to this building.</param>
        /// <param name="accessReason">The reason why this building needs access.</param>
        /// <returns>Returns true if the building can be accessed.</returns>
        public bool AccessableFromAreaId(int areaId, AccessReason accessReason)
        {
            if (accessReason == AccessReason.Construction)
            {
                List<Point> accessPoints = ConstructionPoints(areaId);
                return (accessPoints.Count > 0);
            }
            return false;
        }

        /// <summary>
        /// Gets all the points that provide access from an area.
        /// </summary>
        /// <param name="areaId">The area ID of the object wanting to access this building.</param>
        /// <param name="accessReason">The reason this building needs access.</param>
        /// <returns>A MovementDestination object that the accessing object can use to path to this building.</returns>
        public MovementDestination<Point> GetAccessablePointsByAreaId(int areaId, AccessReason accessReason)
        {
            // TODO: Change the position to the middle of the building if its bigger than 2x2
            switch (accessReason)
            {
                case AccessReason.Construction:
                    return new MovementDestination<Point>(ConstructionPoints(areaId));
                case AccessReason.PlaceItem:
                    if (_buildingState == BuildingStates.UnderConstruction)
                        return new MovementDestination<Point>(ConstructionPoints(areaId));
                    break;
            }
            return null;
        }
    }
}
