using Heliopolis.World.Environment;

namespace Heliopolis.Utilities.EdgeTracking
{
    public class Edge
    {
        //Note edge always points clockwise, A to B
        public NodePoint A;
        public NodePoint B;
        public EnvironmentTile LeftTile;
        public EnvironmentTile RightTile;

        public void Reverse()
        {
            NodePoint swapNode = A;
            A = B;
            B = swapNode;
            EnvironmentTile swapTile = LeftTile;
            LeftTile = RightTile;
            RightTile = swapTile;
        }
    }
}