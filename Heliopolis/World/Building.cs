using System;
using System.Collections.Generic;
using System.Xml;
using Heliopolis.World.Environment;
using Microsoft.Xna.Framework;
using Heliopolis.Utilities;

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
    public class Building : GameWorldObject, System.ICloneable, ICanHoldItem, ICanAccess
    {
        private BuildingTile[,] buildingTiles;
        private List<string> requiredMaterials;
        private List<int> requiredMaterialAmount;
        private List<Item> constructionItems;
        private Point size;
        private string buildingType;
        private Point position;
        private BuildingStates buildingState;
        private Point mainAccessPoint;
        private bool usesMainAccessPoint;
        private Dictionary<Point, BuildingTileContains> itemsHeld;

        /// <summary>
        /// Initialises a new instance of the Building class.
        /// </summary>
        /// <param name="name">The name of the building.</param>
        /// <param name="_size">The size in tiles of the building.</param>
        /// <param name="_buildingTiles">A 2D array containing all the building tiles.</param>
        /// <param name="_requiredMaterials">The materials required for building.</param>
        /// <param name="_requiredMaterialAmounts">The quantities of the materials.</param>
        /// <param name="_owner">The owning game world.</param>
        public Building(string name, Point _size, BuildingTile[,] _buildingTiles, List<string> _requiredMaterials, List<int> _requiredMaterialAmounts, GameWorld _owner) : base (_owner)
        {
            buildingType = name;
            size = _size;
            buildingTiles = _buildingTiles;
            requiredMaterials = _requiredMaterials;
            requiredMaterialAmount = _requiredMaterialAmounts;
            constructionItems = new List<Item>();
            buildingState = BuildingStates.None;
        }

        /// <summary>
        /// The size of the building.
        /// </summary>
        public Point Size
        {
            get { return size; }
        }

        /// <summary>
        /// The actual tiles that make up this building.
        /// </summary>
        public BuildingTile[,] BuildingTiles
        {
            get { return buildingTiles; }
            set { buildingTiles = value; }
        }

        /// <summary>
        /// The required materials.
        /// </summary>
        public List<string> RequiredMaterials
        {
            get { return requiredMaterials; }
        }

        /// <summary>
        /// The required material quantites.
        /// </summary>
        public List<int> RequiredMaterialAmount
        {
            get { return requiredMaterialAmount; }
        }

        /// <summary>
        /// The position in the game world of the top left tile of this building.
        /// </summary>
        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// The current state of this building.
        /// </summary>
        public BuildingStates BuildingState
        {
            get { return buildingState; }
            set { buildingState = value; }
        }

        /// <summary>
        /// The access point for actors to interact with this building.
        /// </summary>
        public Point MainAccessPoint
        {
            get { return mainAccessPoint; }
            set { mainAccessPoint = value; }
        }

        /// <summary>
        /// If this building actually has a main access point.
        /// </summary>
        public bool UsesMainAccessPoint
        {
            get { return usesMainAccessPoint; }
            set { usesMainAccessPoint = value; }
        }

        /// <summary>
        /// All the items in this building.
        /// </summary>
        public Dictionary<Point, BuildingTileContains> ItemsHeld
        {
            get { return itemsHeld; }
            set { itemsHeld = value; }
        }

        #region ICloneable Members

        /// <summary>
        /// Creates a copy of this building.
        /// </summary>
        /// <returns>A Building copy.</returns>
        public object Clone()
        {
            Building returnMe = (Building) MemberwiseClone();
            returnMe.constructionItems = new List<Item>();
            returnMe.itemsHeld = new Dictionary<Point, BuildingTileContains>();
            itemsHeld = new Dictionary<Point, BuildingTileContains>();
            foreach (BuildingTile tile in buildingTiles)
            {
                if (tile.ItemSpace != 0)
                    itemsHeld.Add(tile.Position, new BuildingTileContains(tile.ItemSpace));
            }
            return returnMe;
        }

        #endregion

        /// <summary>
        /// Starts construction of this building.
        /// </summary>
        public void StartBuilding()
        {
            buildingState = BuildingStates.UnderConstruction;
            foreach (BuildingTile buildingTile in buildingTiles)
            {
                EnvironmentTile targetTile = Owner.Environment[position.X + buildingTile.Position.X, position.Y + buildingTile.Position.Y];
                targetTile.CanAccess = false;
            }
        }

        /// <summary>
        /// Completes construction of this building.
        /// </summary>
        public void CompleteBuilding()
        {
            buildingState = BuildingStates.Ready;
            foreach (BuildingTile buildingTile in buildingTiles)
            {
                EnvironmentTile targetTile = Owner.Environment[position.X + buildingTile.Position.X, position.Y + buildingTile.Position.Y];
                targetTile.CanAccess = buildingTile.CanAccess;
            }
        }

        /// <summary>
        /// Checks to see if this building can hold a certain item.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="reserveSpace"></param>
        /// <returns></returns>
        public bool canHold(string itemType, bool reserveSpace)
        {
            foreach (BuildingTileContains container in itemsHeld.Values)
            {
                if (container.CanHold(itemType))
                {
                    container.itemType = itemType;
                    container.spaceReserved++;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks to see if this building has a particular item.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="reserveItem"></param>
        /// <returns></returns>
        public bool hasItem(string itemType, bool reserveItem)
        {
            foreach (BuildingTileContains container in itemsHeld.Values)
            {
                if (container.HasItem(itemType))
                {
                    if (reserveItem)
                        container.itemsReserved++;
                    return true;
                }
            }
            return false;
        }

        #region ICanHoldItem Members

        /// <summary>
        /// Place an item into this building.
        /// </summary>
        /// <param name="item">The item to place.</param>
        public void PickupItem(Item item)
        {
            if (buildingState == BuildingStates.UnderConstruction)
            {
                constructionItems.Add(item);
                item.Holder = this;
                item.ItemState = ItemStates.BeingCarried;
            }
            else if(buildingState == BuildingStates.Ready)
            {
                foreach (BuildingTileContains container in itemsHeld.Values)
                {
                    if (container.CanHold(item.ItemType))
                    {
                        container.AddItem(item);
                        item.Holder = this;
                        item.ItemState = ItemStates.BeingCarried;
                    }
                }
            }
        }

        /// <summary>
        /// Give an item to another ICanHoldItem.
        /// </summary>
        /// <param name="itemHolder">The ICanHoldItem to give the object to.</param>
        /// <param name="itemType">The item to give.</param>
        public void PlaceItem(ICanHoldItem itemHolder, Item item)
        {
            if (buildingState == BuildingStates.Ready)
            {
                foreach (BuildingTileContains container in itemsHeld.Values)
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

        /// <summary>
        /// Put an item on the ground.
        /// </summary>
        public void PlaceItemOnGround(Item itemToDrop)
        {
            throw new Exception("A building can not place an item on the ground.");
        }

        #endregion

        /// <summary>
        /// The points of access that actors can get to, for constrution of this building.
        /// </summary>
        /// <param name="areaID">The area that the accessing actor is in.</param>
        /// <returns>A List of Point where the actor can go to access this building.</returns>
        public List<Point> ConstructionPoints(int areaID)
        {
            List<Point> returnMe = new List<Point>();
            for (int i = 0; i < size.X; i++)
            {
                Point yPosAbove = new Point(position.X + i,position.Y - 1);
                if (yPosAbove.Y >= 0)
                    if (Owner.Environment[yPosAbove].CanAccess && Owner.Environment[yPosAbove].AreaID == areaID)
                        returnMe.Add(yPosAbove);
                Point yPosBelow = new Point(position.X + i, position.Y + size.Y);
                if (yPosBelow.Y <= Owner.Environment.WorldSize.Y)
                    if (Owner.Environment[yPosBelow].CanAccess && Owner.Environment[yPosBelow].AreaID == areaID)
                        returnMe.Add(yPosBelow);
            }
            for (int i = 0; i < size.Y; i++)
            {
                Point xPosLeft = new Point(position.X - 1, position.Y + i);
                if (xPosLeft.X >= 0)
                    if (Owner.Environment[xPosLeft].CanAccess && Owner.Environment[xPosLeft].AreaID == areaID)
                        returnMe.Add(xPosLeft);
                Point xPosRight = new Point(position.X + size.X-1, position.Y + i);
                if (xPosRight.X <= Owner.Environment.WorldSize.X)
                    if (Owner.Environment[xPosRight].CanAccess && Owner.Environment[xPosRight].AreaID == areaID)
                        returnMe.Add(xPosRight);
            }
            return returnMe;
        }

        /// <summary>
        /// Checks to see if this building can be access from a certain area.
        /// </summary>
        /// <param name="areaID">The ID of the area of the object wanting access to this building.</param>
        /// <param name="accessReason">The reason why this building needs access.</param>
        /// <returns>Returns true if the building can be accessed.</returns>
        public bool AccessableFromAreaID(int areaID, AccessReason accessReason)
        {
            if (accessReason == AccessReason.Construction)
            {
                List<Point> accessPoints = ConstructionPoints(areaID);
                return (accessPoints.Count > 0);
            }
            return false;
        }

        /// <summary>
        /// Gets all the points that provide access from an area.
        /// </summary>
        /// <param name="areaID">The area ID of the object wanting to access this building.</param>
        /// <param name="accessReason">The reason this building needs access.</param>
        /// <returns>A MovementDestination object that the accessing object can use to path to this building.</returns>
        public MovementDestination<Point> GetAccessablePointsByAreaID(int areaID, AccessReason accessReason)
        {
            // TODO: Change the position to the middle of the building if its bigger than 2x2
            if (accessReason == AccessReason.Construction)
            {
                return new MovementDestination<Point>(ConstructionPoints(areaID));
            }
            else if (accessReason == AccessReason.PlaceItem)
            {
                if (buildingState == BuildingStates.UnderConstruction)
                    return new MovementDestination<Point>(ConstructionPoints(areaID));
            }
            return null;
        }
    }
}
