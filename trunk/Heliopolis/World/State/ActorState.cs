using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Heliopolis.Utilities;

namespace Heliopolis.World.State
{
    /// <summary>
    /// The base class that all actor states inherit from.
    /// </summary>
    /// <remarks>This abstract class provides all ancestors a number of management tools to manange an actor state.
    /// ActorStates are also able to contain a number of sub states that get processed in the order they are added.
    /// An ancestor state should:
    /// <list type="bullet"><item><description>Set the actionType so that the actor knows how long to spend till the next tick.</description></item>
    /// <item><description>Override the Tick method if the state is to perform an action.</description></item>
    /// <item><description>Hook the checkFinishedState into a method that lets the ActorState class know when this state has completed. If this is not done, the state will run indefinitely.</description></item>
    /// </list></remarks>
    /// <example>
    /// An example of an ancestor state with no substates:
    /// <code>public class ActorStateMove : ActorState
    ///{
    ///private MovementDestination movementDestination;
    ///public ActorStateMove(Actor _myActor, Point _pointToMoveTo, GameWorld _owner)
    ///    : base(_myActor, _owner)
    ///{
    ///    actionType = "movement";
    ///    movementDestination = new MovementDestination(_pointToMoveTo);
    ///    checkFinishedState = checkMoveDone;
    ///}
    ///public override void OnEnter()
    ///{
    ///    myActor.DestinationPosition = movementDestination;
    ///    base.OnEnter();
    ///}
    ///public override void Tick()
    ///{
    ///    myActor.moveNextDirection();
    ///    base.Tick();
    ///}
    ///private bool checkMoveDone()
    ///{
    ///   return (myActor.Directions.Count == 0);
    ///}
    ///}</code>
    /// </example>
    [Serializable]
    public abstract class ActorState : GameWorldObject
    {
        /// <summary>
        /// Delegate type for a method to run across substates
        /// </summary>
        protected delegate void RunOnSubStates();
        /// <summary>
        /// The actor that this state belongs to.
        /// </summary>
        protected Actor myActor;
        /// <summary>
        /// A list of substates, to be processed in order (FIFO).
        /// </summary>
        protected  LinkedList<ActorState> subStates = null;
        /// <summary>
        /// The action type.
        /// </summary>
        protected string actionType;
        /// <summary>
        /// A method to be set up by inheriting states to check if the state has finished.
        /// </summary>
        protected abstract bool checkFinishedState();

        private bool firstSubstateEntered = false;
        private bool stateFinished;

        /// <summary>
        /// Initialises a new instance of the ActorState class.
        /// </summary>
        /// <param name="_myActor">The actor who this state belongs to.</param>
        /// <param name="_owner">The owning game world.</param>
        public ActorState(Actor _myActor, GameWorld _owner) : base(_owner)
        {
            myActor = _myActor;
        }

        /// <summary>
        /// Returns true if this state has finished its tasks.
        /// </summary>
        public bool StateFinished
        {
            get { return stateFinished; }
        }

        /// <summary>
        /// The current action type of this state. Returns a substate's action type if it is active.
        /// </summary>
        public string ActionType
        {
            get
            {
                if (subStates != null)
                {
                    if (subStates.Count > 0)
                    {
                        return subStates.First.Value.ActionType;
                    }
                }
                return actionType;
            }
        }

        /// <summary>
        /// Adds a substate into this state.
        /// </summary>
        /// <param name="addState">An ActorState to add.</param>
        protected void AddSubState(ActorState addState)
        {
            if (subStates == null)
                subStates = new LinkedList<ActorState>();
            subStates.AddLast(addState);
        }

        /// <summary>
        /// Run a method on the first substate. This will also remove a substate
        /// from the list if it has finished.
        /// </summary>
        /// <param name="runOnSubStates">The method to run.</param>
        private void ExecuteAcrossSubStates(RunOnSubStates runOnSubStates)
        {
            runOnSubStates();
            if (subStates.Count > 0)
            {
                while (subStates.First.Value.StateFinished)
                {
                    subStates.RemoveFirst();
                    if (subStates.Count > 0)
                    {
                        subStates.First.Value.OnEnter();
                    }
                    else
                        break;
                }
            }
        }

        /// <summary>
        /// Executes OnEnter events.
        /// </summary>
        public virtual void OnEnter()
        {
            if (subStates != null)
            {
                if (subStates.Count > 0)
                {
                    ExecuteAcrossSubStates(subStates.First.Value.OnEnter);
                }
            }
            checkStateDone();
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public virtual void Tick()
        {
            if (subStates != null)
            {
                if (subStates.Count > 0)
                {
                    if (!firstSubstateEntered)
                    {
                        ExecuteAcrossSubStates(subStates.First.Value.OnEnter);
                        firstSubstateEntered = true;
                    }
                    ExecuteAcrossSubStates(subStates.First.Value.Tick);
                }
            }
            checkStateDone();
        }

        private void checkStateDone()
        {
            if (checkFinishedState())
            {
                stateFinished = true;
            }
        }
    }
}
