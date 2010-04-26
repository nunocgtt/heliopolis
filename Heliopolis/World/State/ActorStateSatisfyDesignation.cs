using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heliopolis.Utilities;
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
        private Designation myDesignation;
        /// <summary>
        /// Initialises a new instance of the ActorStateSatisfyDesignation class.
        /// </summary>
        /// <param name="_myActor">The actor who this state belongs to.</param>
        /// <param name="_myDesignation">The designation to satisfy.</param>
        /// <param name="_owner">The owning game world.</param>
        public ActorStateSatisfyDesignation(Actor _myActor, Designation _myDesignation, GameWorld _owner)
            : base(_myActor, _owner)
        {
            myDesignation = _myDesignation;
            subStates = new LinkedList<ActorState>();
            List<ActorState> subStatesFromDesignation = myDesignation.GetStateStepsToPerform();
            foreach (ActorState substate in subStatesFromDesignation)
                subStates.AddFirst(substate);
        }

        protected override bool checkFinishedState()
        {
            // Designation is done when we have a.moved and b.performed the job
            if (subStates.Count == 0)
            {
                myDesignation.CompleteDesignation();
            }
            return (subStates.Count == 0);
        }
    }
}
