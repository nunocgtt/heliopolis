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
    public static class BuildingFactory
    {
        [NonSerialized]
        private static Dictionary<string, BuildingTemplate> _buildingTemplates;
        private static GameWorld _owner;

        public static Dictionary<string, BuildingTemplate> BuildingTemplates
        {
            get
            {
                return _buildingTemplates;
            }
        }

        public static void LoadTemplatesFromXml(ContentManager contentManager, GameWorld owner)
        {
            _buildingTemplates = contentManager.Load<List<BuildingTemplate>>(@"GameWorldDefintion/buildings").ToDictionary(p => p.Name);
            _owner = owner;
        }

        /// <summary>
        /// Returns a copy of a building template.
        /// </summary>
        /// <param name="templateName">The template to copy.</param>
        /// <param name="position">The top left building tile position in the game world.</param>
        /// <returns>A Building.</returns>
        public static Building GetNewBuilding(string templateName, Point position)
        {
            BuildingTemplate template = _buildingTemplates[templateName];
            Building returnMe = new Building(template.Size, null, null, _owner);
            returnMe.Position = position;
            returnMe.Id = new Guid();
            return returnMe;
        }
    }
}
