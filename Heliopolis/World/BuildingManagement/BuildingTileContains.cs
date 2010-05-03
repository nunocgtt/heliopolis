using System;
using System.Collections.Generic;
using Heliopolis.World.ItemManagement;

namespace Heliopolis.World.BuildingManagement
{
    /// <summary>
    /// Represents a building tile that is able to store items.
    /// </summary>
    /// <remarks>A single building tile can only store one type of item. Also, certain items can be reserved by actors
    /// so that when they come to fulfill a designation that requires movement of the item, it is guaranteed to exist.</remarks>
    [Serializable]
    public class BuildingTileContains
    {
        public string ItemType;
        public int Amount;
        public readonly int MaxAmount;
        public int ItemsReserved;
        public int SpaceReserved;
        public List<Item> ItemsHeld = new List<Item>();

        /// <summary>
        /// Initialises a new instance of the BuildingTileContains class.
        /// </summary>
        /// <param name="maxAmount">The maximum number of items storable.</param>
        public BuildingTileContains(int maxAmount)
        {
            ItemType = "";
            Amount = 0;
            MaxAmount = maxAmount;
            ItemsReserved = 0;
            SpaceReserved = 0;
        }

        /// <summary>
        /// Checks to see if this container is able to hold a specific item type.
        /// </summary>
        /// <param name="itemTypeToHold">The item to place in.</param>
        /// <returns>Returns true if this tile can hold the item.</returns>
        public bool CanHold(string itemTypeToHold)
        {
            return ((ItemType == "" && Amount == 0) || (ItemType == itemTypeToHold && Amount < (MaxAmount - SpaceReserved)));
        }

        /// <summary>
        /// Checks to see if this tile contains an item.
        /// </summary>
        /// <param name="itemTypeRequired">The type of item to check for.</param>
        /// <returns>Returns true if there is an available item.</returns>
        public bool HasItem(string itemTypeRequired)
        {
            return (ItemType == itemTypeRequired && (Amount - ItemsReserved) > 0);
        }

        /// <summary>
        /// Adds an item into this container.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(Item item)
        {
            ItemType = item.ItemType;
            ItemsHeld.Add(item);
            Amount++;
        }

        /// <summary>
        /// Removes an item from this container.
        /// </summary>
        /// <returns>Returns the removed Item.</returns>
        public Item RemoveItem()
        {
            Item returnMe = ItemsHeld[ItemsHeld.Count - 1];
            ItemsHeld.RemoveAt(ItemsHeld.Count - 1);
            if (Amount == 0 && SpaceReserved == 0)
                ItemType = "";
            return returnMe;
        }
    }
}