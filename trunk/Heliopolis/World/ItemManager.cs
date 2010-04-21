using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Heliopolis.Utilities;

namespace Heliopolis.World
{

    /// <summary>
    /// Provides access to all in game items, and manages creation of items.
    /// </summary>
    [Serializable]
    public class ItemManager
    {
        private Dictionary<Point, List<Item>> items = new Dictionary<Point, List<Item>>();

        private GameWorld owner;

        /// <summary>
        /// Initialises a new instance of the ItemManager class.
        /// </summary>
        /// <param name="_owner">The owning game world.</param>
        public ItemManager(GameWorld _owner)
        {
            owner = _owner;
        }

        /// <summary>
        /// Creates an item on the ground.
        /// </summary>
        /// <param name="itemType">The item type to spawn.</param>
        /// <param name="position">The position of the item.</param>
        public void SpawnItem(string itemType, Point position)
        {
            Item toAdd = ItemFactory.GetNewItem(itemType, position);
            owner.SpatialTreeIndex.AddToSection(position, toAdd, SpatialObjectType.Item, itemType);
        }

        /// <summary>
        /// Find an item in an area.
        /// </summary>
        /// <param name="searcherAreaID">The area ID of the searcher.</param>
        /// <param name="itemType">The type of item required.</param>
        /// <param name="searcherPosition">The position of the searcher.</param>
        /// <returns>Returns an item if one exists, otherwise returns null.</returns>
        public Item GetClosestItem(int searcherAreaID, string itemType, Point searcherPosition)
        {
            //TODO: Incorporate the searcher Area ID
            return (Item)owner.SpatialTreeIndex.FindClosestObject(searcherPosition, itemType);
        }

        /// <summary>
        /// Checks to see if a particular item exists in the game world.
        /// </summary>
        /// <param name="searcherAreaID">The area ID of the searcher.</param>
        /// <param name="itemType">The type of item to search for.</param>
        /// <returns>Returns true if at least one item exists.</returns>
        public bool ValidItemExists(int searcherAreaID, string itemType)
        {
            if (this.owner.SpatialTreeIndex.TopNode.ResourceCount.ContainsKey(itemType))
                if (this.owner.SpatialTreeIndex.TopNode.ResourceCount[itemType] > 0)
                    return true;
            return false;
        }

    }
}
