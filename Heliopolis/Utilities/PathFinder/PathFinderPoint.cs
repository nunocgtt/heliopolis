using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities
{
   public class PathFinderPoint : PathFinder<Point>
    {
       public override float GoalDistanceEstimate(Point position, Point goalPosition)
       {
           float XDist = position.X - goalPosition.X;
           float YDist = position.Y - goalPosition.Y;
           return (XDist * XDist) + (YDist * YDist);
       }
    }
}
