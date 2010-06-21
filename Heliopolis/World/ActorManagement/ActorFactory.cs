using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Heliopolis.World.ItemManagement;
using Microsoft.Xna.Framework;
using ContentClasses;
using Microsoft.Xna.Framework.Content;

namespace Heliopolis.World
{
    /// <summary>
    /// Handles creation of the Actor class, from a set of templates defined in an XML file.
    /// </summary>
    public class ActorFactory : IFactory
    {
        [NonSerialized]
        private Dictionary<string, ActorTemplate> _actorTemplates;
        private GameWorld _owner;

        public void LoadTemplatesFromContent(ContentManager contentManager, GameWorld owner, string contentFile)
        {
            _actorTemplates = contentManager.Load<List<ActorTemplate>>(contentFile).ToDictionary(p => p.Name);
            _owner = owner;
        }

        private ActorFactory()
        {
        }

        public static ActorFactory Instance;

        static ActorFactory()
        {
            Instance = new ActorFactory();
        }

        /// <summary>
        /// Creates a new copy of an actor from a particular template.
        /// </summary>
        /// <param name="templateName">The name of the template.</param>
        /// <param name="intialPosition">The initial starting position on the game world.</param>
        /// <returns>Returns an Actor.</returns>
        public Actor GetNewActor(string templateName, Point intialPosition)
        {
            ActorTemplate template = _actorTemplates[templateName];
            Actor returnMe = new Actor(_owner, template.Name, template.Texture, template.Hitpoints, template.Properties.ToDictionary(p => p, q => 0), template.Jobs.ToList());
            returnMe.Start(intialPosition);
            return returnMe;
        }

    }
}