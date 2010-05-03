using System;
using System.Collections.Generic;
using Heliopolis.Utilities.SpatialTreeIndexSystem;
using Microsoft.Xna.Framework;

namespace Heliopolis.World.ItemManagement
{

    /// <summary>
    /// Provides access to all in game items, and manages creation of items.
    /// </summary>
    [Serializable]
    public class ItemManager : GameWorldObject
    {
        private Dictionary<Point, List<Item>> _items = new Dictionary<Point, List<Item>>();

        /// <summary>
        /// Initialises a new instance of the ItemManager class.
        /// </summary>
        /// <param name="owner">The owning game world.</param>
        public ItemManager(GameWorld owner) : base(owner)
        {
        }

        /// <summary>
        /// Creates an item on the ground.
        /// </summary>
        /// <param name="itemType">The item type to spawn.</param>
        /// <param name="position">The position of the item.</param>
        public void SpawnItem(string itemType, Point position)
        {
            Item toAdd = ItemFactory.GetNewItem(itemType, position);
            Owner.SpatialTreeIndex.AddToSection(position, toAdd, SpatialObjectType.Item, itemType);
        }

        /// <summary>
        /// Find an item in an area.
        /// </summary>
        /// <param name="searcherAreaId">The area ID of the searcher.</param>
        /// <param name="itemType">The type of item required.</param>
        /// <param name="searcherPosition">The position of the searcher.</param>
        /// <returns>Returns an item if one exists, otherwise returns null.</returns>
        public Item GetClosestItem(int searcherAreaId, string itemType, Point searcherPosition)
        {
            //TODO: Incorporate the searcher Area ID
            return (Item)Owner.SpatialTreeIndex.FindClosestObject(searcherPosition, itemType);
        }

        public static void PlaceItem(ICanHoldItem source, ICanHoldItem target, Item item, ItemStates newItemState)
        {
            source.PutdownItem(item);
            item.Holder = target;
            item.ItemState = target.PickupItem(item);
        }

        public static void OnItemCreate(ICanHoldItem target, Item item)
        {
            item.ItemState = target.PickupItem(item);
            item.Holder = target;
        }

        /// <summary>
        /// Checks to see if a particular item exists in the game world.
        /// </summary>
        /// <param name="searcherAreaId">The area ID of the searcher.</param>
        /// <param name="itemType">The type of item to search for.</param>
        /// <returns>Returns true if at least one item exists.</returns>
        public bool ValidItemExists(int searcherAreaId, string itemType)
        {
            if (this.Owner.SpatialTreeIndex.TopNode.ResourceCount.ContainsKey(itemType))
                if (this.Owner.SpatialTreeIndex.TopNode.ResourceCount[itemType] > 0)
                    return true;
            return false;
        }

    }
}
