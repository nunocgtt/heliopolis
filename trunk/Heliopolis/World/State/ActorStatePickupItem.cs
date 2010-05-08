using System;
using Heliopolis.Utilities.PathFinder;
using Microsoft.Xna.Framework;
using Heliopolis.World.ItemManagement;

namespace Heliopolis.World.State
{
    /// <summary>
    /// Represents an actor in a state of movement,
    /// </summary>
    [Serializable]
    public class ActorStatePickupItem : ActorState
    {
        public Item ItemToPickup { get; set; }

        public ActorStatePickupItem(Actor myActor, Item itemToPickup, GameWorld owner)
            : base(myActor, owner)
        {
            ActionType = "pickup";
            ItemToPickup = itemToPickup;
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public override void Tick()
        {
            //TODO: Pick up the item
            ItemManager.PlaceItem(ItemToPickup.Holder, MyActor, ItemToPickup);
            base.Tick();
        }
    }
}