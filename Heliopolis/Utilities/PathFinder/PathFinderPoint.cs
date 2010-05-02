using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities.PathFinder
{
   public class PathFinderPoint : PathFinder<Point>
    {
       public override float GoalDistanceEstimate(Point position, Point goalPosition)
       {
           float xDist = position.X - goalPosition.X;
           float yDist = position.Y - goalPosition.Y;
           return (xDist * xDist) + (yDist * yDist);
       }
    }
}
