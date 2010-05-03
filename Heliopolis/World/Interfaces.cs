using Heliopolis.Utilities.PathFinder;
using Microsoft.Xna.Framework;

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
    /// For tiles that can be accessed.
    /// </summary>
    public interface ICanAccess
    {
        /// <summary>
        /// Checks to see if this ICanAccess can be accessed from a particular area.
        /// </summary>
        /// <param name="areaId">The area ID of the accessor.</param>
        /// <param name="accessReason">Why the access is required.</param>
        /// <returns>Returns true if this can be access.</returns>
        bool AccessableFromAreaId(int areaId, AccessReason accessReason);
        /// <summary>
        /// Destination information for pathing, to access from a particular area.
        /// </summary>
        /// <param name="areaId">The area ID of the accessor.</param>
        /// <param name="accessReason">hy the access is required.</param>
        /// <returns>A MovementDestination detailling how to get to this ICanAccess.</returns>
        MovementDestination<Point> GetAccessablePointsByAreaId(int areaId, AccessReason accessReason);
    }

}
