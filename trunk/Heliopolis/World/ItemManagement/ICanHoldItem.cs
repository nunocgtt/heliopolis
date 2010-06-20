using Heliopolis.Utilities.SpatialTreeIndexSystem;

namespace Heliopolis.World.ItemManagement
{
    /// <summary>
    /// To be implemented by objects that need to pick up and place items.
    /// </summary>
    public interface ICanHoldItem : ISpatialIndexMember
    {
        /// <summary>
        /// Pick up an item and place into inventory/storage.
        /// </summary>
        /// <param name="itemToPickup"></param>
        ItemStates PickupItem(Item itemToPickup);

        /// <summary>
        /// Put down an item.
        /// </summary>
        /// <param name="itemToPlace">The item to give.</param>
        void PutdownItem(Item itemToPlace);
    }
}