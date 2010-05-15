using System;
using Heliopolis.World.ItemManagement;

namespace Heliopolis.World.State
{
    /// <summary>
    /// Represents the state of picking up an item from the item's owner.
    /// </summary>
    [Serializable]
    public class ActorStatePlaceItem : ActorState
    {
        public ICanHoldItem NewItemHolder { get; set; }

        public ActorStatePlaceItem(Actor myActor, ICanHoldItem newItemHolder, GameWorld owner)
            : base(myActor, owner, true)
        {
            NewItemHolder = newItemHolder;
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
            foreach (var item in MyActor.InHand)
            {
                ItemManager.PlaceItem(MyActor, NewItemHolder, item);   
            }
            Finished = true;
        }
    }
}