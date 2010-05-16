using System;
using System.Linq;
using Heliopolis.World.ItemManagement;

namespace Heliopolis.World.State
{
    /// <summary>
    /// Represents the state of picking up an item from the item's owner.
    /// </summary>
    [Serializable]
    public class ActorStateDropHeldItem : ActorState
    {

        public ActorStateDropHeldItem(Actor myActor, GameWorld owner)
            : base(myActor, owner, true)
        {
            ActionType = "pickupitem";
        }

        public override void OnEnter()
        {
            
        }

        public override void OnFinish()
        {
            
        }

        public override void Tick()
        {
            foreach (var item in MyActor.InHand.ToList())
            {
                ItemManager.PlaceItem(MyActor, Owner.Environment[MyActor.Position], item);   
            }
            Finished = true;
        }

    }
}