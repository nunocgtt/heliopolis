using System;
using System.Collections.Generic;
using System.Linq;
using Heliopolis.World.Environment;
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

        #region ICanAccess Members

        public IEnumerable<Point> GetAllAccessPoints()
        {
            return OwningTile.AdjacentTiles.Select(environmentTile => environmentTile.Position).ToList();
        }

        #endregion
    }
}