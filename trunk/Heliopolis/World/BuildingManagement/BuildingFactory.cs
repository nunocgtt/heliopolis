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

        /// <summary>
        /// Returns a copy of a building template.
        /// </summary>
        /// <param name="templateName">The template to copy.</param>
        /// <param name="position">The top left building tile position in the game world.</param>
        /// <returns>A Building.</returns>
        public Building GetNewBuilding(string templateName, Point position)
        {
            BuildingTemplate template = _buildingTemplates[templateName];
            Building returnMe = new Building(template.Size, null, null, _owner);
            returnMe.Position = position;
            returnMe.Id = new Guid();
            return returnMe;
        }
    }
}
