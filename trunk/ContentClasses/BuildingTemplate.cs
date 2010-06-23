using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ContentClasses
{
    public class BuildingTemplate
    {
        public string Name { get; set; }
        public Point Size { get; set; }
        public List<BulidingTileTemplate> Tiles { get; set; }
        public List<BuildingMaterialTemplate> RequiredMaterials { get; set; }
    }

    public class BulidingTileTemplate
    {
        public Point Position { get; set; }
        public bool CanAccess { get; set; }
        public string Texture { get; set; }
        public string InteractableObjectName { get; set; }
    }

    public class BuildingMaterialTemplate
    {
        public string ItemName { get; set; }
        public int Count { get; set; }
    }
}