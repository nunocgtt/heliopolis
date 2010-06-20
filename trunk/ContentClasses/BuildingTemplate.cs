using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ContentClasses
{
    public class BuildingTemplate
    {
        public string Name { get; set; }
        public Point Size { get; set; }
        public List<BulidingTile> Tiles { get; set; }
    }

    public class BulidingTile
    {
        public Point Position { get; set; }
        public string TileType { get; set; }
    }
}