using System;
using Heliopolis.Utilities.PathFinder;
using Microsoft.Xna.Framework;
using Heliopolis.World.ItemManagement;

namespace Heliopolis.World.State
{
    /// <summary>
    /// Represents the state of picking up an item from the item's owner.
    /// </summary>
    [Serializable]
    public class ActorStateTakeItemFromOwner : ActorState
    {
        public Item ItemToPickup { get; set; }

        public ActorStateTakeItemFromOwner(Actor myActor, Item itemToPickup, GameWorld owner)
            : base(myActor, owner, true)
        {
            ActionType = "pickupitem";
            ItemToPickup = itemToPickup;
        }

        public override void OnEnter()
        {
            
        }

        public override void OnFinish()
        {
            
        }

        public override void Tick()
        {
            ItemManager.PlaceItem(ItemToPickup.Holder, MyActor, ItemToPickup);
            Finished = true;
        }
    }
}