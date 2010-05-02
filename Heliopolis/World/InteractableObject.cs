using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heliopolis.World.Environment;

namespace Heliopolis.World
{
    public class InteractableObject : TimedEventor
    {
        public EnvironmentTile OwningTile { get; set; }
        public string Texture { get; set; }
        public List<string> Actions = new List<string>();

        protected InteractableObject(GameWorld owner, EnvironmentTile owningTile, string texture,
            string action)
            : base(owner)
        {
            OwningTile = owningTile;
            Texture = texture;
            Actions.Add(action);
        }

        public override void ExecuteTick(TimeSpan absoluteMilliseconds)
        {
            // do nothing atm
        }
    }

    public class HarvestableInteractableObject : InteractableObject
    {
        public string ResourceType { get; set; }
        public int ResourceCount { get; set; }

        public HarvestableInteractableObject(GameWorld owner, EnvironmentTile owningTile, string texture,
            int resourceCount, string resourceType, string action)
            : base(owner, owningTile, texture, action)
        {
            OwningTile = owningTile;
            TimedEventDisabled = true;
            Texture = texture;
            ResourceType = resourceType;
            ResourceCount = resourceCount;
            TimedEventDisabled = true;
        }

        public override void ExecuteTick(TimeSpan absoluteMilliseconds)
        {
            // do nothing atm
        }
    }

    public class ManufactureInteractableObject : InteractableObject
    {
        public ManufactureInteractableObject(GameWorld owner, EnvironmentTile owningTile, string texture,
            string action)
            : base(owner, owningTile, texture, action)
        {
            OwningTile = owningTile;
            TimedEventDisabled = true;
            Texture = texture;
            TimedEventDisabled = true;
        }
    }
}