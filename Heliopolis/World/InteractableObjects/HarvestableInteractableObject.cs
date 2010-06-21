using System;
using System.Collections.Generic;
using System.Linq;
using Heliopolis.World.Environment;
using Heliopolis.World.ItemManagement;
using Microsoft.Xna.Framework;

namespace Heliopolis.World.InteractableObjects
{
    public class HarvestableInteractableObject : InteractableObject, ICanAccess
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

        public void Harvest(out Item itemHarvested)
        {
            itemHarvested = ItemFactory.Instance.GetNewItem(ResourceType);
            ResourceCount--;
            if (ResourceCount == 0)
            {
                OwningTile.RemoveInteractableObject();
            }
        }


        #region ICanAccess Members

        public IEnumerable<Point> GetAllAccessPoints()
        {
            return OwningTile.GetAdjacentAccessPoints();
        }

        #endregion
    }
}