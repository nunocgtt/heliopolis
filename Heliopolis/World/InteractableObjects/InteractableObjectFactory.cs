using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentClasses;
using Microsoft.Xna.Framework.Content;

namespace Heliopolis.World.InteractableObjects
{
    public class InteractableObjectFactory : IFactory
    {
        private Dictionary<string, InteractableObjectTemplate> _interactableTemplates;
        private GameWorld _owner;

        public void LoadTemplatesFromContent(ContentManager contentManager, GameWorld owner, string contentFile)
        {
            _interactableTemplates = contentManager.Load<List<InteractableObjectTemplate>>(contentFile).ToDictionary(p => p.Name);
            _owner = owner;
        }

        private InteractableObjectFactory()
        {
        }

        public static InteractableObjectFactory Instance;

        static InteractableObjectFactory()
        {
            Instance = new InteractableObjectFactory();
        }
    }
}
