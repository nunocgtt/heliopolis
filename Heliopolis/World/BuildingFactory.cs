using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{
    /// <summary>
    /// Handles creation of the Building class, from a set of templates defined in an XML file.
    /// </summary>
    public class BuildingFactory
    {
        [NonSerialized]
        private static Dictionary<string, Building> buildingTemplates = null;

        /// <summary>
        /// All the building templates.
        /// </summary>
        public static Dictionary<string, Building> BuildingTemplates
        {
            get { return buildingTemplates; }
        }

        /// <summary>
        /// Loads Building templates from an XML file.
        /// </summary>
        /// <param name="xmlDoc">The xml file.</param>
        /// <param name="owner">The owning game world.</param>
        public static void LoadTemplatesFromXml(XmlDocument xmlDoc, GameWorld owner)
        {
            buildingTemplates = new Dictionary<string, Building>();
            XmlNodeList itemNodes = xmlDoc.GetElementsByTagName("Building");
            foreach (XmlNode node in itemNodes)
            {
                XmlNode Xsize = node.SelectSingleNode("XSize");
                XmlNode Ysize = node.SelectSingleNode("YSize");
                XmlNodeList tileNodes = node.SelectNodes("tile");
                XmlNodeList resources = node.SelectNodes("BuildingResource");
                Point buildingSize = new Point(Int32.Parse(Xsize.InnerText), Int32.Parse(Ysize.InnerText));
                BuildingTile[,] buildingTile = new BuildingTile[buildingSize.X, buildingSize.Y];
                int x = 0;
                int y = 0;
                bool usesMainAccesPoint = false;
                Point mainAccesPoint = new Point(0, 0);
                foreach (XmlNode n in tileNodes)
                {
                    XmlNode noAccess = n.SelectSingleNode("noaccess");
                    XmlNode mainAccessPointNode = n.SelectSingleNode("mainaccesspoint");
                    XmlNode carryNode = n.SelectSingleNode("holds");
                    XmlNode textureNode = n.SelectSingleNode("Texture");
                    BuildingTile addTile = new BuildingTile();
                    addTile.Texture = textureNode.InnerText;
                    if (noAccess != null)
                        addTile.CanAccess = false;
                    else
                        addTile.CanAccess = true;
                    if (mainAccessPointNode != null)
                    {
                        usesMainAccesPoint = true;
                        mainAccesPoint = new Point(x, y);
                    }
                    if (carryNode != null)
                    {
                        addTile.ItemSpace = Int32.Parse(carryNode.InnerText);
                    }
                    else
                        addTile.ItemSpace = 0;
                    addTile.Position = new Point(x, y);
                    buildingTile[x, y] = addTile;
                    x++;
                    if (x >= buildingSize.X)
                    {
                        x = 0;
                        y++;
                    }
                }
                List<string> requiredMaterials = new List<string>();
                List<int> requiredMaterialAmount = new List<int>();
                foreach (XmlNode n in resources)
                {
                    requiredMaterials.Add(n.Attributes["name"].Value);
                    requiredMaterialAmount.Add(Int32.Parse(n.Attributes["quantity"].Value));
                }
                Building addBuilding = new Building(node.Attributes["name"].Value, buildingSize, buildingTile, requiredMaterials, requiredMaterialAmount, owner);
                addBuilding.MainAccessPoint = mainAccesPoint;
                addBuilding.UsesMainAccessPoint = usesMainAccesPoint;
                AddTemplate(node.Attributes["name"].Value, addBuilding);
            }
        }

        /// <summary>
        /// Returns a copy of a building template.
        /// </summary>
        /// <param name="templateName">The template to copy.</param>
        /// <param name="position">The top left building tile position in the game world.</param>
        /// <returns>A Building.</returns>
        public static Building GetNewBuilding(string templateName, Point position)
        {
            Building returnMe = (Building)BuildingTemplates[templateName].Clone();
            returnMe.Position = position;
            returnMe.Id = new Guid();
            return returnMe;
        }

        private static void AddTemplate(string name, Building addBuilding)
        {
            buildingTemplates.Add(name, addBuilding);
        }
    }
}
