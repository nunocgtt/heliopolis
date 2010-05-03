using System.Collections.Generic;
using Heliopolis.Utilities.PathFinder;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{
    /// <summary>
    /// For tiles that can be accessed.
    /// </summary>
    public interface ICanAccess
    {
        /// <summary>
        /// Destination information for pathing, to access from a particular area.
        /// </summary>
        /// <returns>A MovementDestination detailing how to get to this ICanAccess.</returns>
        IEnumerable<Point> GetAllAccessPoints();
    }
}