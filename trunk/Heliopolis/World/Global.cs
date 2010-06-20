using Heliopolis.Utilities;
using Heliopolis.Utilities.PathFinder;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{
    public static class Global
    {
        public static PathFinderPoint PathFinder = new PathFinderPoint();
        public static FillFinder<Point> FillFinder = new FillFinder<Point>();
    }
}
