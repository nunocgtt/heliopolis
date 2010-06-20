using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using ContentClasses;
using Microsoft.Xna.Framework.Content;
using System.Linq;

namespace Heliopolis.World.ItemManagement
{

    /// <summary>
    /// Handles creation of the Item class, from a set of templates defined in an XML file.
    /// </summary>
    public static class ItemFactory
    {
        [NonSerialized]
        private static Dictionary<string, ItemTemplate> _itemTemplates = null;
        private static GameWorld _owner;

        public static void LoadTemplatesFromXml(ContentManager contentManager, GameWorld owner)
        {
            _itemTemplates = contentManager.Load<List<ItemTemplate>>(@"GameWorldDefintion/items").ToDictionary(p => p.Name);
            _owner = owner;
        }

        /// <summary>
        /// Creates a new copy of an Item template.
        /// </summary>
        /// <param name="templateName">The name of the template to copy.</param>
        /// <param name="position">The position to create the item at.</param>
        /// <returns>An Item.</returns>
        public static Item GetNewItem(string templateName, Point position)
        {
            ItemTemplate itemTemplate = _itemTemplates[templateName];
            Item returnMe = new Item(itemTemplate.Weight,itemTemplate.Texture, itemTemplate.Name, _owner);
            returnMe.Position = position;
            return returnMe;
        }

        public static Item GetNewItem(string templateName)
        {
            ItemTemplate itemTemplate = _itemTemplates[templateName];
            Item returnMe = new Item(itemTemplate.Weight, itemTemplate.Texture, itemTemplate.Name, _owner);
            return returnMe;
        }
    }
}
