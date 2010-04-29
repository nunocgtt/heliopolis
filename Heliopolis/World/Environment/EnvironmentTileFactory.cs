using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{

    /// <summary>
    /// Handles creation of the EnvironmentTile class, from a set of templates defined in an XML file.
    /// </summary>
    public class EnvironmentTileFactory
    {
        private static Dictionary<string, EnvironmentTile> tileTemplates = null;

        /// <summary>
        /// Loads EnvironmentTile templates from an XML file.
        /// </summary>
        /// <param name="xmlDoc">The xml file.</param>
        /// <param name="owner">The owning game world.</param>
        public static void LoadTemplatesFromXml(XmlDocument xmlDoc, GameWorld owner)
        {
            tileTemplates = new Dictionary<string, EnvironmentTile>();
            XmlNodeList tileNodes = xmlDoc.GetElementsByTagName("TileType");
            foreach (XmlNode node in tileNodes)
            {
                XmlNode texture = node.SelectSingleNode("Texture");
                XmlNode canAcess = node.SelectSingleNode("CanAccess");
                // Note some of these nodes are optional
                EnvironmentTile addTile = new EnvironmentTile(
                    texture.InnerText,
                    (canAcess != null),
                    owner);
                AddTemplate(node.Attributes["name"].Value, addTile);
            }
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
            EnvironmentTile returnMe = (EnvironmentTile)tileTemplates[templateName].Clone();
            returnMe.Id = new Guid();
            returnMe.Position = newPosition;
            returnMe.AdjacentTiles = new List<EnvironmentTile>(4);
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

        /// <summary>
        /// Changes a tile into another template.
        /// </summary>
        /// <param name="templateName">The template to change to.</param>
        /// <param name="loadInto">The EnvironmentTile to change.</param>
        public static void SetToTemplate(string templateName, EnvironmentTile loadInto)
        {
            loadInto.Texture = tileTemplates[templateName].Texture;
            loadInto.CanAccess = tileTemplates[templateName].CanAccess;
        }

        private static void AddTemplate(string name, EnvironmentTile addTile)
        {
            tileTemplates.Add(name, addTile);
        }
    }
}
