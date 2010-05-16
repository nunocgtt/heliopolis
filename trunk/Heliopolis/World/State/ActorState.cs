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
        /// The actor that this state belongs to.
        /// </summary>
        protected readonly Actor MyActor;
        /// <summary>
        /// The action type.
        /// </summary>
        public string ActionType;

        public bool Entered;
        private bool _finished;
        public bool RequiresTime;
        public bool RootState;

        protected ActorState(Actor myActor, GameWorld owner, bool requiresTime) : base(owner)
        {
            MyActor = myActor;
            RequiresTime = requiresTime;
        }

        public bool Finished
        {
            get
            {
                if (RootState)
                    return false;
                return _finished;
            }
            set { _finished = value; }
        }

        public abstract void OnEnter();

        public abstract void OnFinish();

        /// <summary>
        /// Ticks this instance.
        /// </summary>
        /// <returns>Return true if this tick takes no time in the game world.</returns>
        public abstract void Tick();
    }
}
