using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heliopolis.Utilities
{
    /// <summary>
    /// Node used for searching. Each node represents a single position in a search grid.
    /// </summary>
    public class Node<T>
    {
        // Note if we ever want to turn this generic, we need to reimlement "position"
        public Node<T> Parent;
        public Node<T> Child;

        /// <summary>
        /// Cost of this node + it's predecessors.
        /// </summary>
        public float G;
        /// <summary>
        /// Heuristic estimate of distance to goal
        /// </summary>
        public float H;
        /// <summary>
        /// sum of cumulative cost of predecessors and self and heuristic
        /// </summary>
        public float F;

        public T Position;

        public Direction ComeFrom;
        public Direction GoTo;

        public Node(T set)
        {
            Position = set;
            G = 0;
            H = 0;
            F = 0;
            Parent = null;
            Child = null;
            ComeFrom = Direction.Nowhere;
            GoTo = Direction.Nowhere;
        }
        public Node(T set, Direction _comeFrom)
        {
            Position = set;
            G = 0;
            H = 0;
            F = 0;
            Parent = null;
            Child = null;
            ComeFrom = _comeFrom;
            GoTo = Direction.Nowhere;
        }

        public override string ToString()
        {
            return Position.ToString() + " g : " + G.ToString() + " h : " + H.ToString() + " f : " + F.ToString();
        }
    }
}
