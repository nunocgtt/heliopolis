using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Heliopolis.World.BuildingManagement;

namespace Heliopolis.World.State
{
    /// <summary>
    /// Actor needs to stash an item in its hand.
    /// </summary>
    [Serializable]
    public class ActorStateStashCurrentItem : ActorState
    {
        /// <summary>
        /// Initialises a new instance of the ActorStateIdle class.
        /// </summary>
        /// <param name="myActor">The actor who this state belongs to.</param>
        /// <param name="owner">The owning game world.</param>
        public ActorStateStashCurrentItem(Actor myActor, GameWorld owner)
            : base(myActor, owner, false)
        {
            ActionType = "movement";
        }

        /// <summary>
        /// Set the actor's destination when this state is entered.
        /// </summary>
        public override void OnEnter()
        {
            Building stashAt = Owner.BuildingManager.FindStorage();
            if (stashAt != null)
            {
                MyActor.State.AddNewSubstate(new ActorStateMoveToICanAccess(MyActor, stashAt, Owner));
                MyActor.State.AddNewSubstate(new ActorStatePlaceItem(MyActor, stashAt, Owner));
            }
            else
            {
                MyActor.State.AddNewSubstate(new ActorStateDropHeldItem(MyActor, Owner));
            }
        }

        public override void OnFinish()
        {
            
        }

        public override void Tick()
        {
            Finished = true;
        }
    }
}
