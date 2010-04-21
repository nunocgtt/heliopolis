using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heliopolis.Utilities
{
    /// <summary>
    /// Information returned from a successful path search.
    /// </summary>
    public class PathfindAnswer
    {
        public LinkedList<Direction> directions;
        public object owner;
    }
}