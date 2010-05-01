using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heliopolis.Utilities;
using Microsoft.Xna.Framework;

namespace Heliopolis.World.State
{
    /// <summary>
    /// Represents an actor in a state of movement,
    /// </summary>
    [Serializable]
    public class ActorStateMove : ActorState
    {
        private MovementDestination<Point> movementDestination;

        /// <summary>
        /// Initialises a new instance of the ActorStateMove class, with a destination point.
        /// </summary>
        /// <param name="_myActor">The actor who this state belongs to.</param>
        /// <param name="_pointToMoveTo">The final position to move to.</param>
        /// <param name="_owner">The owning game world.</param>
        public ActorStateMove(Actor _myActor, Point _pointToMoveTo, GameWorld _owner)
            : base(_myActor, _owner)
        {
            actionType = "movement";
            movementDestination = new MovementDestination<Point>(_pointToMoveTo);
        }

        /// <summary>
        /// Initialises a new instance of the ActorStateMove class, with a MovementDestination destination.
        /// </summary>
        /// <param name="_myActor">The actor who this state belongs to.</param>
        /// <param name="_movementDestination">A MovementDestination instance containing destination information.</param>
        /// <param name="_owner">The owning game world.</param>
        public ActorStateMove(Actor _myActor, MovementDestination<Point> _movementDestination, GameWorld _owner)
            : base(_myActor, _owner)
        {
            actionType = "movement";
            movementDestination = _movementDestination;
        }

        /// <summary>
        /// Set the actor's destination when this state is entered.
        /// </summary>
        public override void OnEnter()
        {
            MyActor.DestinationPosition = movementDestination;
            base.OnEnter();
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public override void Tick()
        {
            MyActor.MoveNextDirection();
            base.Tick();
        }

        protected override bool CheckFinishedState()
        {
            return (MyActor.Directions.Count == 0);
        }
    }
}
