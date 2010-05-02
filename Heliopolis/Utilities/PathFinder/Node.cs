namespace Heliopolis.Utilities.PathFinder
{
    /// <summary>
    /// Node used for searching. Each node represents a single position in a search grid.
    /// </summary>
    public class Node<T>
    {
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
        /// Sum of cumulative cost of predecessors and self and heuristic
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
        public Node(T set, Direction comeFrom)
        {
            Position = set;
            G = 0;
            H = 0;
            F = 0;
            Parent = null;
            Child = null;
            ComeFrom = comeFrom;
            GoTo = Direction.Nowhere;
        }

        public override string ToString()
        {
            return Position + " g : " + G + " h : " + H + " f : " + F;
        }
    }
}
