using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities
{
   public  class PathFinderPoint : PathFinder<Point>
    {
       public override float GoalDistanceEstimate(Point position, Point goalPosition)
       {
           Point pos = (Point)position;
           Point goal = (Point)goalPosition;
           float XDist = pos.X - goal.X;
           float YDist = pos.Y - goal.Y;
           return (XDist * XDist) + (YDist * YDist);
       }
    }
}
