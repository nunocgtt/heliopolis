using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{

    /// <summary>
    /// Handles creation of the Item class, from a set of templates defined in an XML file.
    /// </summary>
    public class ItemFactory
    {
        [NonSerialized]
        private static Dictionary<string, Item> _itemTemplates = null;

        /// <summary>
        /// Loads Item templates from an XML file.
        /// </summary>
        /// <param name="xmlDoc">The xml file.</param>
        /// <param name="owner">The owning game world.</param>
        public static void LoadTemplatesFromXml(XmlDocument xmlDoc, GameWorld owner)
        {
            _itemTemplates = new Dictionary<string, Item>();
            XmlNodeList itemNodes = xmlDoc.GetElementsByTagName("Item");
            foreach (XmlNode node in itemNodes)
            {
                XmlNode weightNode = node.SelectSingleNode("Weight");
                XmlNode texture = node.SelectSingleNode("Texture");
                XmlNode classification = node.SelectSingleNode("Class");
                Item addItem = new Item(
                    float.Parse(weightNode.InnerText),
                    classification.InnerText,
                    texture.InnerText,
                    node.Attributes["name"].Value,
                    owner);
                AddTemplate(node.Attributes["name"].Value, addItem);
            }
        }

        /// <summary>
        /// Creates a new copy of an Item template.
        /// </summary>
        /// <param name="templateName">The name of the template to copy.</param>
        /// <param name="position">The position to create the item at.</param>
        /// <returns>An Item.</returns>
        public static Item GetNewItem(string templateName, Point position)
        {
            Item returnMe = (Item)_itemTemplates[templateName].Clone();
            returnMe.Position = position;
            return returnMe;
        }

        private static void AddTemplate(string name, Item addItem)
        {
            _itemTemplates.Add(name, addItem);
        }
    }
}
