using System;
using System.Collections.Generic;
using Heliopolis.Utilities.PathFinder;
using Heliopolis.World.Environment;

namespace Heliopolis.World.InteractableObjects
{
    public class InteractableObject : TimedEventor
    {
        public EnvironmentTile OwningTile { get; set; }
        public string Texture { get; protected set; }
        public string Action { get; set; }

        protected InteractableObject(GameWorld owner, EnvironmentTile owningTile, string texture,
            string action)
            : base(owner)
        {
            OwningTile = owningTile;
            Texture = texture;
            Action = action;
        }

        public override void ExecuteTick(TimeSpan absoluteMilliseconds)
        {
            // do nothing atm
        }
    }
}