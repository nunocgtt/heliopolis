using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ContentClasses;
using Heliopolis.World.ItemManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Heliopolis.World.Environment
{

    /// <summary>
    /// Handles creation of the EnvironmentTile class, from a set of templates defined in an XML file.
    /// </summary>
    public class EnvironmentTileFactory
    {
        private static Dictionary<string, TileTemplate> _tileTemplates;
        private static GameWorld _owner;

        public static void LoadTemplatesFromXml(ContentManager contentManager, GameWorld owner)
        {
            _tileTemplates = contentManager.Load<List<TileTemplate>>(@"GameWorldDefintion/tiles").ToDictionary(p => p.Name);
            _owner = owner;
        }

        /// <summary>
        /// Creates a new copy of an EnvironmentTile from a particular template.
        /// </summary>
        /// <param name="templateName">The name of the template.</param>
        /// <param name="newPosition">The position of the tile.</param>
        /// <returns>A new EnvironmentTile.</returns>
        public static EnvironmentTile GetNewTile(string templateName,
            Point newPosition)
        {
            TileTemplate template = _tileTemplates[templateName];
            EnvironmentTile returnMe = new EnvironmentTile(template.Texture, template.CanAccess, _owner);
            returnMe.Id = new Guid();
            returnMe.Position = newPosition;
            returnMe.AdjacentTiles = new List<EnvironmentTile>(4);
            returnMe.ItemsOnGround = new List<Item>();
            if (returnMe.CanAccess)
                returnMe.AreaID = 0;
            else
                returnMe.AreaID = -1;
            for (int i = 0; i < 4; i++)
            {
                returnMe.AdjacentTiles.Add(null);
            }
            returnMe.ActorsOnTile = new List<Actor>();
            return returnMe;
        }

    }
}
