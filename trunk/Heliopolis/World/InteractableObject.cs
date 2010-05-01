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

        // Manufacturer InteractableObject

        public InteractableObject(GameWorld _owner, EnvironmentTile _owningTile, string _texture,
            string action)
            : base(_owner)
        {
            this.OwningTile = _owningTile;
            this.Texture = _texture;
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

        public HarvestableInteractableObject(GameWorld _owner, EnvironmentTile _owningTile, string _texture,
            int resourceCount, string resourceType, string action)
            : base(_owner, _owningTile, _texture, action)
        {
            this.OwningTile = _owningTile;
            this.TimedEventDisabled = true;
            this.Texture = _texture;
            this.ResourceType = resourceType;
            this.ResourceCount = resourceCount;
            this.TimedEventDisabled = true;
        }

        public override void ExecuteTick(TimeSpan absoluteMilliseconds)
        {
            // do nothing atm
        }
    }

    public class ManufactureInteractableObject : InteractableObject
    {
        public ManufactureInteractableObject(GameWorld _owner, EnvironmentTile _owningTile, string _texture,
            string action)
            : base(_owner, _owningTile, _texture, action)
        {
            this.OwningTile = _owningTile;
            this.TimedEventDisabled = true;
            this.Texture = _texture;
            this.TimedEventDisabled = true;
        }
    }
}