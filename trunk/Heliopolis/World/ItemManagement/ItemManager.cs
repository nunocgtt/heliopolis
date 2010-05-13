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
        /// <summary>
        /// Initialises a new instance of the ItemManager class.
        /// </summary>
        /// <param name="owner">The owning game world.</param>
        public ItemManager(GameWorld owner) : base(owner)
        {
        }

        public void SpawnItem(string itemType, ICanHoldItem holder)
        {
            Item toAdd = ItemFactory.GetNewItem(itemType);
            toAdd.Holder = holder;
            toAdd.ItemState = holder.PickupItem(toAdd);
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
            SpatialObjectKey spatialObjectKey = new SpatialObjectKey(){ ObjectType = SpatialObjectType.Item, ObjectSubtype = itemType};
            return (Item)Owner.SpatialTreeIndex.FindClosestObject(searcherPosition, spatialObjectKey);
        }

        public static void PlaceItem(ICanHoldItem source, ICanHoldItem target, Item item)
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
            var spatialObjectKey = new SpatialObjectKey() { ObjectType = SpatialObjectType.Item, ObjectSubtype  = itemType};
            if (Owner.SpatialTreeIndex.TopNode.ResourceCount.ContainsKey(spatialObjectKey))
                if (Owner.SpatialTreeIndex.TopNode.ResourceCount[spatialObjectKey] > 0)
                    return true;
            return false;
        }

    }
}
