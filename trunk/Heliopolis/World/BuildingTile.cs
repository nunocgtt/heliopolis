using System;
using System.Collections.Generic;
using System.Text;
using Heliopolis.World.ItemManagement;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{
    /// <summary>
    /// Enumeration of the various states of a building tile.
    /// </summary>
    public enum BuildingTileType
    {
        /// <summary>
        /// Tile is able to store items.
        /// </summary>
        Storage,
        /// <summary>
        /// Tile is able to facilitate item construction.
        /// </summary>
        Construction,
        /// <summary>
        /// Tile does nothing.
        /// </summary>
        Nothing
    }

    /// <summary>
    /// Represents a single tile of a building.
    /// </summary>
    [Serializable]
    public class BuildingTile
    {
        /// <summary>
        /// The type of the tile.
        /// </summary>
        public BuildingTileType BuildingTileType;
        /// <summary>
        /// If this tile is physically accessable.
        /// </summary>
        public bool CanAccess;
        /// <summary>
        /// The texture of this tile.
        /// </summary>
        public string Texture;
        /// <summary>
        /// The position of this tile relative to the top left of the building.
        /// </summary>
        public Point Position;
        /// <summary>
        /// The amount of item space.
        /// </summary>
        public int ItemSpace = 0;
    }

    /// <summary>
    /// Represents a building tile that is able to store items.
    /// </summary>
    /// <remarks>A single building tile can only store one type of item. Also, certain items can be reserved by actors
    /// so that when they come to fulfill a designation that requires movement of the item, it is guaranteed to exist.</remarks>
    [Serializable]
    public class BuildingTileContains
    {
        public string itemType;
        public int amount;
        public int maxAmount;
        public int itemsReserved;
        public int spaceReserved;
        public List<Item> itemsHeld = new List<Item>();

        /// <summary>
        /// Initialises a new instance of the BuildingTileContains class.
        /// </summary>
        /// <param name="_maxAmount">The maximum number of items storable.</param>
        public BuildingTileContains(int _maxAmount)
        {
            itemType = "";
            amount = 0;
            maxAmount = _maxAmount;
            itemsReserved = 0;
            spaceReserved = 0;
        }

        /// <summary>
        /// Checks to see if this container is able to hold a specific item type.
        /// </summary>
        /// <param name="itemTypeToHold">The item to place in.</param>
        /// <returns>Returns true if this tile can hold the item.</returns>
        public bool CanHold(string itemTypeToHold)
        {
            return ((itemType == "" && amount == 0) || (itemType == itemTypeToHold && amount < (maxAmount - spaceReserved)));
        }

        /// <summary>
        /// Checks to see if this tile contains an item.
        /// </summary>
        /// <param name="itemTypeRequired">The type of item to check for.</param>
        /// <returns>Returns true if there is an available item.</returns>
        public bool HasItem(string itemTypeRequired)
        {
            return (itemType == itemTypeRequired && (amount - itemsReserved) > 0);
        }

        /// <summary>
        /// Adds an item into this container.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(Item item)
        {
            itemType = item.ItemType;
            itemsHeld.Add(item);
            amount++;
        }

        /// <summary>
        /// Removes an item from this container.
        /// </summary>
        /// <returns>Returns the removed Item.</returns>
        public Item RemoveItem()
        {
            Item returnMe = itemsHeld[itemsHeld.Count - 1];
            itemsHeld.RemoveAt(itemsHeld.Count - 1);
            if (amount == 0 && spaceReserved == 0)
                itemType = "";
            return returnMe;
        }
    }
}
