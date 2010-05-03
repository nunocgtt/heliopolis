using System;
using System.Collections.Generic;
using System.Xml;
using Heliopolis.World.ItemManagement;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{
    /// <summary>
    /// Handles creation of the Actor class, from a set of templates defined in an XML file.
    /// </summary>
    public class ActorFactory
    {
        [NonSerialized]
        private static Dictionary<string, Actor> _actorTemplates = null;

        /// <summary>
        /// Loads Actor templates from an XML file.
        /// </summary>
        /// <param name="xmlDoc">The xml file.</param>
        /// <param name="owner">The owning game world.</param>
        public static void LoadTemplatesFromXml(XmlDocument xmlDoc, GameWorld owner)
        {
            _actorTemplates = new Dictionary<string, Actor>();
            XmlNodeList actorNodes = xmlDoc.GetElementsByTagName("Actor");
            foreach (XmlNode node in actorNodes)
            {
                XmlNode texture = node.SelectSingleNode("Texture");
                //XmlNode movementSpeed = node.SelectSingleNode("MovementSpeed");
                XmlNode hitpoints = node.SelectSingleNode("Hitpoints");
                XmlNodeList propertyNodes = node.SelectNodes("Property");
                XmlNodeList jobNodes = node.SelectNodes("Job");
                Dictionary<string, int> property = new Dictionary<string, int>();
                List<string> jobs = new List<string>();
                //List<int> magnitude = new List<int>();
                if (propertyNodes != null)
                    foreach (XmlNode n in propertyNodes)
                    {
                        property.Add(n.Attributes["name"].Value, int.Parse(n.Attributes["magnitude"].Value));
                    }
                if (jobNodes != null)
                    foreach (XmlNode n in jobNodes)
                    {
                        jobs.Add(n.InnerText);
                    }
                Actor addActor = new Actor(
                    owner,
                    node.Attributes["name"].Value,
                    texture.InnerText,
                    int.Parse(hitpoints.InnerText),
                    property,
                    jobs);
                AddTemplate(node.Attributes["name"].Value, addActor);
            }
        }

        /// <summary>
        /// Creates a new copy of an actor from a particular template.
        /// </summary>
        /// <param name="templateName">The name of the template.</param>
        /// <param name="intialPosition">The initial starting position on the game world.</param>
        /// <returns>Returns an Actor.</returns>
        public static Actor GetNewActor(string templateName, Point intialPosition)
        {
            Actor returnMe = (Actor)_actorTemplates[templateName].Clone();
            returnMe.Id = new Guid();
            returnMe.Position = intialPosition;
            returnMe.ActionTimes.Add("movement", TimeSpan.FromMilliseconds(200));
            returnMe.ActionTimes.Add("mining", TimeSpan.FromMilliseconds(300));
            returnMe.ActionTimes.Add("idle", TimeSpan.FromMilliseconds(100));
            returnMe.ActionTimes.Add("pickupitem", TimeSpan.FromMilliseconds(100));
            returnMe.ActionTimes.Add("placeitem", TimeSpan.FromMilliseconds(100));
            returnMe.ActionTimes.Add("construction", TimeSpan.FromMilliseconds(100));
            returnMe.InHand = new List<Item>();
            returnMe.Inventory = new List<Item>();
            returnMe.Start();
            return returnMe;
        }

        private static void AddTemplate(string name, Actor addActor)
        {
            _actorTemplates.Add(name, addActor);
        }
    }
}