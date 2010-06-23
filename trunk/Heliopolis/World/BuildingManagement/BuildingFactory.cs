using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using ContentClasses;
using Microsoft.Xna.Framework.Content;

namespace Heliopolis.World.BuildingManagement
{
    /// <summary>
    /// Handles creation of the Building class, from a set of templates defined in an XML file.
    /// </summary>
    public class BuildingFactory : IFactory
    {
        [NonSerialized]
        private Dictionary<string, BuildingTemplate> _buildingTemplates;
        private GameWorld _owner;

        public Dictionary<string, BuildingTemplate> BuildingTemplates
        {
            get
            {
                return _buildingTemplates;
            }
        }

        public void LoadTemplatesFromContent(ContentManager contentManager, GameWorld owner, string contentFile)
        {
            _buildingTemplates = contentManager.Load<List<BuildingTemplate>>(contentFile).ToDictionary(p => p.Name);
            _owner = owner;
        }

        private BuildingFactory()
        {
        }

        public static BuildingFactory Instance;

        static BuildingFactory()
        {
            Instance = new BuildingFactory();
        }

        public Building GetNewBuilding(string templateName)
        {
            BuildingTemplate template = _buildingTemplates[templateName];
            BuildingTile[,] tiles = new BuildingTile[template.Size.X, template.Size.Y];
            for (int i = 0; i < template.Size.X; i++)
            {
                for (int j = 0; j < template.Size.Y; j++)
                {
                    BulidingTileTemplate tileTemplate = template.Tiles.First(p => p.Position == new Point(i, j));
                    tiles[i, j] = new BuildingTile()
                    {
                        BuildingTileType = BuildingTileType.Construction,
                        CanAccess = tileTemplate.CanAccess,
                        ItemSpace = 0,
                        Position = new Point(i, j),
                        Texture = tileTemplate.Texture
                    };
                }
            }
            List<string> requiredMats = new List<string>();
            foreach (var buildingMaterialTemplate in template.RequiredMaterials)
            {
                for (int i = 0; i < buildingMaterialTemplate.Count; i++)
                {
                    requiredMats.Add(buildingMaterialTemplate.ItemName);
                }
            }
            Building returnMe = new Building(template.Size, tiles, requiredMats, _owner);
            returnMe.Id = new Guid();
            return returnMe;
        }


        public Building GetNewBuilding(string templateName, Point position)
        {
            Building returnMe = GetNewBuilding(templateName);
            returnMe.Position = position;
            return returnMe;
        }
    }
}
