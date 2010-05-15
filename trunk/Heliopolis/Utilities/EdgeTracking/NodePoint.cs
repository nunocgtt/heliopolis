using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities.EdgeTracking
{
    public class NodePoint
    {
        public Point Position;

        public NodePoint(Point position)
        {
            Position = position;
        }
        public Edge GoingUp;
        public Edge GoingLeft;
        public Edge GoingRight;
        public Edge GoingDown;
    }
}