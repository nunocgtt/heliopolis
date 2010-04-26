using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heliopolis.World.State
{
    /// <summary>
    /// Represents an actor idle state where the actor looks for designations to complete.
    /// </summary>
    [Serializable]
    public class ActorStateIdle : ActorState
    {
        /// <summary>
        /// Initialises a new instance of the ActorStateIdle class.
        /// </summary>
        /// <param name="_myActor">The actor who this state belongs to.</param>
        /// <param name="_owner">The owning game world.</param>
        public ActorStateIdle(Actor _myActor, GameWorld _owner)
            : base(_myActor, _owner)
        {
            actionType = "idle";
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public override void Tick()
        {
            foreach (string s in myActor.JobsAble)
            {
                Designation someDesignation;
                
                if (owner.DesignationManager.CheckAvailableDesignation(myActor.AreaID, s, myActor.Position, out someDesignation))
                {
                    someDesignation.AssignDesignation(myActor);
                    myActor.State = new ActorStateSatisfyDesignation(myActor, someDesignation, owner);
                    break;
                }
            }
            base.Tick();
        }

        // Idle will create new states to replace itself, so it will never "finish".
        protected override bool checkFinishedState()
        {
            return false;
        }
    }
}
