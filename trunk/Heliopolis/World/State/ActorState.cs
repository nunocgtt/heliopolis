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
        protected Actor MyActor;
        /// <summary>
        /// A list of substates, to be processed in order (FIFO).
        /// </summary>
        protected  LinkedList<ActorState> SubStates = null;
        /// <summary>
        /// The action type.
        /// </summary>
        protected string ActionType;
        /// <summary>
        /// A method to be set up by inheriting states to check if the state has finished.
        /// </summary>
        protected abstract bool CheckFinishedState();

        private bool _firstSubstateEntered = false;

        /// <summary>
        /// Initialises a new instance of the ActorState class.
        /// </summary>
        /// <param name="myActor">The actor who this state belongs to.</param>
        /// <param name="owner">The owning game world.</param>
        protected ActorState(Actor myActor, GameWorld owner) : base(owner)
        {
            MyActor = myActor;
        }

        /// <summary>
        /// Returns true if this state has finished its tasks.
        /// </summary>
        public bool StateFinished { get; private set; }

        /// <summary>
        /// The current action type of this state. Returns a substate's action type if it is active.
        /// </summary>
        public string CurrentActionType
        {
            get
            {
                if (SubStates != null)
                {
                    if (SubStates.Count > 0)
                    {
                        return SubStates.First.Value.CurrentActionType;
                    }
                }
                return ActionType;
            }
        }

        /// <summary>
        /// Adds a substate into this state.
        /// </summary>
        /// <param name="addState">An ActorState to add.</param>
        protected void AddSubState(ActorState addState)
        {
            if (SubStates == null)
                SubStates = new LinkedList<ActorState>();
            SubStates.AddLast(addState);
        }

        /// <summary>
        /// Run a method on the first substate. This will also remove a substate
        /// from the list if it has finished.
        /// </summary>
        /// <param name="runOnSubStates">The method to run.</param>
        private void ExecuteAcrossSubStates(RunOnSubStates runOnSubStates)
        {
            runOnSubStates();
            if (SubStates.Count <= 0) return;
            while (SubStates.First.Value.StateFinished)
            {
                SubStates.RemoveFirst();
                if (SubStates.Count > 0)
                {
                    SubStates.First.Value.OnEnter();
                }
                else
                    break;
            }
        }

        /// <summary>
        /// Executes OnEnter events.
        /// </summary>
        public virtual void OnEnter()
        {
            if (SubStates != null)
            {
                if (SubStates.Count > 0)
                {
                    ExecuteAcrossSubStates(SubStates.First.Value.OnEnter);
                }
            }
            CheckStateDone();
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public virtual void Tick()
        {
            if (SubStates != null)
            {
                if (SubStates.Count > 0)
                {
                    if (!_firstSubstateEntered)
                    {
                        ExecuteAcrossSubStates(SubStates.First.Value.OnEnter);
                        _firstSubstateEntered = true;
                    }
                    ExecuteAcrossSubStates(SubStates.First.Value.Tick);
                }
            }
            CheckStateDone();
        }

        private void CheckStateDone()
        {
            if (CheckFinishedState())
            {
                StateFinished = true;
            }
        }
    }
}
