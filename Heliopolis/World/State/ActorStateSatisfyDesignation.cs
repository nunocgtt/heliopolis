using System;
using System.Collections.Generic;
using Heliopolis.World.JobSystem;

namespace Heliopolis.World.State
{
    /// <summary>
    /// Represents an actor state designed to satisfy a designation.
    /// </summary>
    /// <remarks>Assumption: The designation should already be a valid one before this state gets assigned.
    /// This state will contain a number of sub states, depending on the type of designation.</remarks>
    [Serializable]
    public class ActorStateSatisfyDesignation : ActorState
    {
        private readonly Designation _myDesignation;
        /// <summary>
        /// Initialises a new instance of the ActorStateSatisfyDesignation class.
        /// </summary>
        /// <param name="myActor">The actor who this state belongs to.</param>
        /// <param name="myDesignation">The designation to satisfy.</param>
        /// <param name="owner">The owning game world.</param>
        public ActorStateSatisfyDesignation(Actor myActor, Designation myDesignation, GameWorld owner)
            : base(myActor, owner, false)
        {
            _myDesignation = myDesignation;
            
        }

        public override void OnEnter()
        {
            MyActor.State.AddListOfSubstates(_myDesignation.GetStateStepsToPerform());
        }

        public override void OnFinish()
        {
            _myDesignation.CompleteDesignation();
        }

        public override void Tick()
        {
            Finished = true;
        }

    }
}
