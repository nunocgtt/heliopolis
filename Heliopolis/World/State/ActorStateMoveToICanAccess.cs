using System;
using System.Linq;
using Heliopolis.Utilities.PathFinder;
using Microsoft.Xna.Framework;

namespace Heliopolis.World.State
{
    /// <summary>
    /// Represents an actor in a state of movement,
    /// </summary>
    [Serializable]
    public class ActorStateMoveToICanAccess : ActorState
    {
        private readonly ICanAccess _movementDestination;

        /// <summary>
        /// Initialises a new instance of the ActorStateMove class, with a MovementDestination destination.
        /// </summary>
        /// <param name="myActor">The actor who this state belongs to.</param>
        /// <param name="movementDestination">A MovementDestination instance containing destination information.</param>
        /// <param name="owner">The owning game world.</param>
        public ActorStateMoveToICanAccess(Actor myActor, ICanAccess movementDestination, GameWorld owner)
            : base(myActor, owner, true)
        {
            _movementDestination = movementDestination;
            ActionType = "movement";
        }

        /// <summary>
        /// Set the actor's destination when this state is entered.
        /// </summary>
        public override void OnEnter()
        {
            MyActor.DestinationPosition =
                new MovementDestination<Point>(
                    _movementDestination.GetAllAccessPoints()
                    .Where(p => Owner.Environment[p].CanAccess)
                    .ToList());
        }

        public override void OnFinish()
        {
            
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public override void Tick()
        {
            MyActor.MoveNextDirection();
            if (MyActor.Directions.Count == 0)
            {
                Finished = true;
            }
        }

    }
}