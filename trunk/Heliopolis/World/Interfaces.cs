using Microsoft.Xna.Framework;
using Heliopolis.Utilities;

namespace Heliopolis.World
{
    /// <summary>
    /// A reason why a building tile needs to be access.
    /// </summary>
    public enum AccessReason
    {
        /// <summary>
        /// For construction of a building.
        /// </summary>
        Construction,
        /// <summary>
        /// To pick up an item.
        /// </summary>
        PickupItem,
        /// <summary>
        /// To place an item.
        /// </summary>
        PlaceItem,
        /// <summary>
        /// To harvest a good.
        /// </summary>
        Harvest
    }

    /// <summary>
    /// To be implemented by objects that need to pick up and place items.
    /// </summary>
    public interface ICanHoldItem
    {
        /// <summary>
        /// Pick up an item and place into inventory/storage.
        /// </summary>
        /// <param name="item"></param>
        void PickupItem(Item item);
        /// <summary>
        /// Give an item to another ICanHoldItem
        /// </summary>
        /// <param name="itemHolder">The ICanHoldItem to give the item to.</param>
        /// <param name="itemToPlace">The item to give.</param>
        void PlaceItem(ICanHoldItem itemHolder, Item itemToPlace);
        /// <summary>
        /// Put an item on the ground.
        /// </summary>
        void PlaceItemOnGround(Item itemToDrop);
        /// <summary>
        /// All ICanHoldItem members must also have a position.
        /// </summary>
        Point Position { get; set; }
    }

    /// <summary>
    /// For tiles that can be accessed.
    /// </summary>
    public interface ICanAccess
    {
        /// <summary>
        /// Checks to see if this ICanAccess can be accessed from a particular area.
        /// </summary>
        /// <param name="areaID">The area ID of the accessor.</param>
        /// <param name="accessReason">Why the access is required.</param>
        /// <returns>Returns true if this can be access.</returns>
        bool AccessableFromAreaID(int areaID, AccessReason accessReason);
        /// <summary>
        /// Destination information for pathing, to access from a particular area.
        /// </summary>
        /// <param name="areaID">The area ID of the accessor.</param>
        /// <param name="accessReason">hy the access is required.</param>
        /// <returns>A MovementDestination detailling how to get to this ICanAccess.</returns>
        MovementDestination<Point> GetAccessablePointsByAreaID(int areaID, AccessReason accessReason);
    }

}
