using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Heliopolis.Utilities;

namespace Heliopolis.World
{
    public class Global
    {
        public static PathFinderPoint PathFinder = new PathFinderPoint();
        public static FillFinder<Point> FillFinder = new FillFinder<Point>();
        public static TraceManager<Point> TraceManager = new TraceManager<Point>();
    }
}
