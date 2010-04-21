using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{
    /// <summary>
    /// General utility methods that don't fall into a particular class.
    /// </summary>
    public static class WorldUtilities
    {
        /// <summary>
        /// Find the closest point from a list of points.
        /// </summary>
        /// <param name="pointList">A list of destinations.</param>
        /// <param name="originPoint">The point searching from.</param>
        /// <returns>The closest point is returned.</returns>
        public static Point closestPoint(List<Point> pointList, Point originPoint)
        {
            Point returnMe = new Point(0, 0);
            int smallestDistance = int.MaxValue;
            foreach (Point p in pointList)
            {
                int xdistance = (p.X - originPoint.X);
                int ydistance = (p.Y - originPoint.Y);
                int distance = (xdistance * xdistance) + (ydistance * ydistance);
                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    returnMe = p;
                }
            }
            return returnMe;
        }

        /// <summary>
        /// Checks if two points are co-located.
        /// </summary>
        /// <param name="onePoint">The position of the first object.</param>
        /// <param name="twoPoint">The position of the second object.</param>
        /// <returns>Returns true if the two supplied positions are next to each other.</returns>
        public static bool colocatedPoints(Point onePoint, Point twoPoint)
        {
            return ((onePoint.X == twoPoint.X - 1) && (onePoint.Y == twoPoint.Y)) ||
                ((onePoint.X == twoPoint.X + 1) && (onePoint.Y == twoPoint.Y)) ||
                ((onePoint.Y == twoPoint.Y - 1) && (onePoint.X == twoPoint.X)) ||
                ((onePoint.Y == twoPoint.Y + 1) && (onePoint.X == twoPoint.X));
        }
    }
}
