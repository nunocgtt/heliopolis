using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heliopolis.World.InteractableObjects;
using Microsoft.Xna.Framework.Content;
using Heliopolis.World.ItemManagement;
using Heliopolis.World.Environment;
using Heliopolis.World.BuildingManagement;

namespace Heliopolis.World
{
    public class FactoryManager : GameWorldObject
    {
        private Dictionary<IFactory, string> _factories;

        public FactoryManager(GameWorld owner) : base(owner)
        {
            _factories = new Dictionary<IFactory, string>();
            _factories.Add(InteractableObjectFactory.Instance, @"GameWorldDefinition/interactableobjects");
            _factories.Add(ActorFactory.Instance, @"GameWorldDefinition/actors");
            _factories.Add(ItemFactory.Instance, @"GameWorldDefinition/items");
            _factories.Add(EnvironmentTileFactory.Instance, @"GameWorldDefinition/tiles");
            _factories.Add(BuildingFactory.Instance, @"GameWorldDefinition/buildings");
            _factories.Add(ActionTimes.Instance, @"GameWorldDefinition/actiontimes");
        }

        public void InitializeFactories(ContentManager contentManager)
        {
            foreach (KeyValuePair<IFactory, string> kvp in _factories)
            {
                kvp.Key.LoadTemplatesFromContent(contentManager, Owner, kvp.Value);
            }
        }
    }
}
