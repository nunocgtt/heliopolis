using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Heliopolis.Utilities;
using Heliopolis.World.State;

namespace Heliopolis.World
{
    /// <summary>
    /// Represents a single entity in the GameWorld that is an actual being.
    /// </summary>
    /// <remarks>This entity has a few characteristics that mark it as being different from a static object.
    /// The first is that it contains an inventory and can hold items. The second is that it has a complex
    /// internal state, which is managed by the ActorState class.
    /// The Actors are also able to fulfill designations.
    /// Actors also retain an internal set of counters, for things like health, hunger, sleep and other needs.</remarks>
    [Serializable]
    public class Actor : TimedEventor, System.ICloneable, ICanHoldItem, ISpatialIndexMember
    {
        private Point position;
        private MovementDestination<Point> destinationPosition;
        private List<Item> inventory;
        private LinkedList<Direction> directions = null;
        private int hitpoints;
        private Dictionary<string, int> properties = null;
        private List<string> jobsAble = null;
        private string texture;
        private ActorState state;
        private Dictionary<string, TimeSpan> actionTimes;
        private string actorType;

        /// <summary>
        /// The current section/area ID of this actor.
        /// </summary>
        public int AreaID
        {
            get { return owner.Environment.GameWorld[position.X, position.Y].AreaID; }
        }

        /// <summary>
        /// The type of this actor.
        /// </summary>
        public string ActorType
        {
            get { return actorType; }
            set { actorType = value; }
        }

        /// <summary>
        /// Initialises a new instance of the Actor class.
        /// </summary>
        /// <param name="_owner">The owning GameWorld.</param>
        /// <param name="_actorType">The actor type.</param>
        /// <param name="_texture">The texture to draw.</param>
        /// <param name="_hitPoints">The starting number of hitpoints.</param>
        /// <param name="_properties">Any internal properties of this actor.</param>
        /// <param name="_jobsAble">The jobs this actor can initially perform.</param>
        public Actor(GameWorld _owner,
            string _actorType,
            string _texture,
            int _hitPoints,
            Dictionary<string, int> _properties,
            List<string> _jobsAble) : base (_owner)
        {
            texture = _texture;
            hitpoints = _hitPoints;
            properties = _properties;
            jobsAble = _jobsAble;
            actorType = _actorType;
            position = new Point(-1, -1);
        }

        /// <summary>
        /// Contains the various actions this actor can peform, and how long it takes to do each.
        /// </summary>
        public Dictionary<string, TimeSpan> ActionTimes
        {
            get { return actionTimes; }
            set { actionTimes = value; }
        }

        /// <summary>
        /// If this actor is moving, this is a list of the directions to take to reach its destination.
        /// </summary>
        public LinkedList<Direction> Directions
        {
            get { return directions; }
        }

        /// <summary>
        /// Contains the current Actor's state.
        /// </summary>
        public ActorState State
        {
            get { return state; }
            set { changeState(value); }
        }

        /// <summary>
        /// When set, this will make the actor determine a new path to the destination position.
        /// </summary>
        public MovementDestination<Point> DestinationPosition
        {
            set
            {
                destinationPosition = value;
                moveToPosition(value);
            }
            get { return destinationPosition; }
        }

        /// <summary>
        /// The texture of this actor to render.
        /// </summary>
        public string Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        /// <summary>
        /// A list of jobs that this actor is able to perform.
        /// </summary>
        public List<string> JobsAble
        {
            get { return jobsAble; }
        }

        /// <summary>
        /// The number of hitpoints of this actor.
        /// </summary>
        public int Hitpoints
        {
            get { return hitpoints; }
            set 
            {
                StateRecorder.Instance.AddStateChange(this.Id, "Hitpoints", typeof(int), hitpoints, value, this.nextAbsoluteActionTime);
                hitpoints = value; 
            }
        }

        /// <summary>
        /// A list of items this actor is currently carrying.
        /// </summary>
        public List<Item> Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }

        /// <summary>
        /// The position of this Actor in the game world.
        /// </summary>
        public Point Position
        {
            get { return position; }
            set
            {
                changePosition(value);
            }
        }

        public bool ShouldBeRendered
        {
            get { return true; }
        }

        /// <summary>
        /// Moves this actor into a new state.
        /// </summary>
        /// <param name="newState">The new state.</param>
        private void changeState(ActorState newState)
        {
            state = newState;
            state.OnEnter();
        }

        #region ICanHoldItem Members

        /// <summary>
        /// Pick up an item and place into this actors inventory.
        /// </summary>
        /// <param name="item">The item to pick up.</param>
        public void PickupItem(Item item)
        {
            inventory.Add(item);
            item.ItemState = ItemStates.BeingCarried;
            item.Holder = this;
        }

        /// <summary>
        /// Place a certain item type on to another ICanHoldItem. **NOT IMPLEMENTED**
        /// </summary>
        /// <param name="itemHolder">The ICanHoldItem to give the item to.</param>
        /// <param name="itemType">The type of item to place.</param>
        public void PlaceItem(ICanHoldItem itemHolder, string itemType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Put an item on the ground at this actors current position.
        /// </summary>
        public void PlaceItemOnGround()
        {
            Item itemToPlace = inventory[0];
            itemToPlace.ItemState = ItemStates.OnGround;
            itemToPlace.Position = this.position;
        }

        /// <summary>
        /// Place a particular item from this actors inventory into another ICanHoldItem.
        /// </summary>
        /// <param name="itemHolder">The ICanHoldItem to give the item to.</param>
        public void PlaceItem(ICanHoldItem itemHolder)
        {
            Item itemToPlace = inventory[0];
            itemHolder.PickupItem(itemToPlace);
            inventory.Remove(itemToPlace);
        }

        #endregion

        /// <summary>
        /// Pathfinding logic to get this actor to the specified position.
        /// </summary>
        /// <param name="newPosition">The new position to move to.</param>
        private void moveToPosition(MovementDestination<Point> newPosition)
        {
            PathFinder<Point> pathFinder = Global.PathFinder;
            if (newPosition.PointToMoveTo != position)
            {
                PathfindRequest<Point> newrequest = null;
                if (newPosition.MovementDestinationType == MovementDestinationType.SinglePoint)
                {
                    newrequest = new PathfindRequest<Point>(position, newPosition.PointToMoveTo, this);
                }
                else if (newPosition.MovementDestinationType == MovementDestinationType.MultiPoint)
                {
                    newrequest = new PathfindRequest<Point>(position, newPosition.PointToMoveTo, this, newPosition.PointsAcceptable);
                }
                else
                    throw new Exception("Can not move to an unaccessable position.");
                pathFinder.NewSearch(newrequest);
                if (pathFinder.SearchStep(999) == SearchState.SEARCH_STATE_SUCCEEDED)
                {
                    PathfindAnswer theAnswer = pathFinder.finalResult();
                    directions = theAnswer.directions;
                }
                else
                    throw new Exception("Unable to path to that position.");
            }
            else
            {
                directions.Clear();
            }
        }

        /// <summary>
        /// Check to see if this actor has moved into a new section/area.
        /// </summary>
        /// <param name="newPosition">The new position to move to.</param>
        private void changePosition(Point newPosition)
        {
            owner.SpatialTreeIndex.CheckChangeSection(position, newPosition, this, SpatialObjectType.Actor,"");
            StateRecorder.Instance.AddStateChange(this.Id, "Position", typeof(Point), position, newPosition, this.nextAbsoluteActionTime);
            position = newPosition;
        }

        /// <summary>
        /// Move this actor in the next direction contained in it's internal Directions list.
        /// </summary>
        public void moveNextDirection()
        {
            if (directions.Count > 0)
            {
                Direction currentDirection = directions.First.Value;
                switch (currentDirection)
                {
                    case Direction.South:
                        changePosition(new Point(position.X,position.Y+1));
                        break;
                    case Direction.North:
                        changePosition(new Point(position.X, position.Y - 1));
                        break;
                    case Direction.West:
                        changePosition(new Point(position.X - 1, position.Y));
                        break;
                    case Direction.East:
                        changePosition(new Point(position.X + 1, position.Y));
                        break;
                    case Direction.Nowhere:
                        break;
                }
                directions.RemoveFirst();
            }
        }

        /// <summary>
        /// Process this actors move.
        /// </summary>
        /// <param name="absoluteMilliseconds">The absolute game time.</param>
        public override void ExecuteTick(TimeSpan absoluteMilliseconds)
        {
            state.Tick();
            if (state.StateFinished)
            {
                state = new ActorStateIdle(this, owner);
            }
            // Want to set up the next tick
            SetUpNextTick(actionTimes[state.ActionType]);
        }

        /// <summary>
        /// Creates a new copy of this class.
        /// </summary>
        /// <returns>An Actor copy.</returns>
        public object Clone()
        {
            Actor returnMe = (Actor)MemberwiseClone();
            returnMe.Inventory = new List<Item>();
            returnMe.ActionTimes = new Dictionary<string, TimeSpan>();
            returnMe.State = new ActorStateIdle(returnMe, owner);
            return returnMe;
        }

    }

    

}
