using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heliopolis.Utilities;
using Heliopolis.World.JobSystem;
using Microsoft.Xna.Framework;

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
            : base(myActor, owner)
        {
            this._myDesignation = myDesignation;
            SubStates = new LinkedList<ActorState>();
            List<ActorState> subStatesFromDesignation = this._myDesignation.GetStateStepsToPerform();
            foreach (ActorState substate in subStatesFromDesignation)
                SubStates.AddFirst(substate);
        }

        protected override bool CheckFinishedState()
        {
            // Designation is done when we have a.moved and b.performed the job
            if (SubStates.Count == 0)
            {
                _myDesignation.CompleteDesignation();
            }
            return (SubStates.Count == 0);
        }
    }
}
