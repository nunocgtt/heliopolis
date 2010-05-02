using System.Collections.Generic;

namespace Heliopolis.Utilities.PathFinder
{
    /// <summary>
    /// Information returned from a successful path search.
    /// </summary>
    public class PathfindAnswer
    {
        public LinkedList<Direction> Directions;
        public object Owner;
    }
}