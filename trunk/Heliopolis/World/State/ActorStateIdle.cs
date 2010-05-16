using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heliopolis.World.JobSystem;

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
        /// <param name="myActor">The actor who this state belongs to.</param>
        /// <param name="owner">The owning game world.</param>
        public ActorStateIdle(Actor myActor, GameWorld owner)
            : base(myActor, owner, true)
        {
            ActionType = "idle";
            RootState = true;
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public override void Tick()
        {
            foreach (string s in MyActor.JobsAble)
            {
                Designation someDesignation;
                
                if (Owner.DesignationManager.CheckAvailableDesignation(MyActor.AreaID, s, MyActor.Position, out someDesignation))
                {
                    someDesignation.AssignDesignation(MyActor);
                    MyActor.State.AddNewSubstate(new ActorStateSatisfyDesignation(MyActor, someDesignation, Owner));
                }
            }
        }

        public override void OnEnter()
        {
            
        }

        public override void OnFinish()
        {

        }
    }
}
