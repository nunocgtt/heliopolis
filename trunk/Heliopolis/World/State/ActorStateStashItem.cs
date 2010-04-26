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
    public class ActorStateStashItem : ActorState
    {
        /// <summary>
        /// Initialises a new instance of the ActorStateIdle class.
        /// </summary>
        /// <param name="_myActor">The actor who this state belongs to.</param>
        /// <param name="_owner">The owning game world.</param>
        public ActorStateStashItem(Actor _myActor, GameWorld _owner)
            : base(_myActor, _owner)
        {
            actionType = "idle";
            subStates.AddLast(new ActorStateMove(_myActor, new Point(0,0), _owner));
            //subStates.AddLast(new ActorStateStashItem
        }

        public override void OnEnter()
        {

            base.OnEnter();
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public override void Tick()
        {
            
        }

        // Idle will create new states to replace itself, so it will never "finish".
        protected override bool checkFinishedState()
        {
            return false;
        }
    }
}
