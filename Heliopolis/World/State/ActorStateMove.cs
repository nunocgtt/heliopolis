﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heliopolis.Utilities;
using Heliopolis.Utilities.PathFinder;
using Microsoft.Xna.Framework;

namespace Heliopolis.World.State
{
    /// <summary>
    /// Represents an actor in a state of movement,
    /// </summary>
    [Serializable]
    public class ActorStateMove : ActorState
    {
        private readonly MovementDestination<Point> _movementDestination;

        /// <summary>
        /// Initialises a new instance of the ActorStateMove class, with a destination point.
        /// </summary>
        /// <param name="myActor">The actor who this state belongs to.</param>
        /// <param name="pointToMoveTo">The final position to move to.</param>
        /// <param name="owner">The owning game world.</param>
        public ActorStateMove(Actor myActor, Point pointToMoveTo, GameWorld owner)
            : base(myActor, owner, true)
        {
            ActionType = "movement";
            _movementDestination = new MovementDestination<Point>(pointToMoveTo);
        }

        /// <summary>
        /// Set the actor's destination when this state is entered.
        /// </summary>
        public override void OnEnter()
        {
            MyActor.DestinationPosition = _movementDestination;
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
