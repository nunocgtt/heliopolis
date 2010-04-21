using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heliopolis.Utilities
{
    /// <summary>
    /// Information required to perform a path finding solution.
    /// </summary>
    public class PathfindRequest<T>
    {
        public T start;
        public T end;
        public object owner;

        public List<T> possibleSolutions = null;

        public PathfindRequest(T _start, T _end, object _owner)
        {
            start = _start;
            end = _end;
            owner = _owner;
        }

        public PathfindRequest(T _start, T _end, object _owner, List<T> _possibleSolutions)
        {
            start = _start;
            end = _end;
            owner = _owner;
            possibleSolutions = _possibleSolutions;
        }

        public override string ToString()
        {
            return " { Start - " + start.ToString() + " End - " + end.ToString() + " Owner - " + owner.ToString();
        }
    }
}
