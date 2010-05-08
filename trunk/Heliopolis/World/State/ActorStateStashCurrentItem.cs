using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Heliopolis.World.State
{
    /// <summary>
    /// Represents an actor idle state where the actor looks for designations to complete.
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
            : base(myActor, owner)
        {
            ActionType = "idle";
            SubStates.AddLast(new ActorStateMove(myActor, new Point(0,0), owner));
            //subStates.AddLast(new ActorStateStashItem
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public override void Tick()
        {
            
        }

        // Idle will create new states to replace itself, so it will never "finish".
        protected override bool CheckFinishedState()
        {
            return false;
        }
    }
}
