#region

using System;
using System.Collections.Generic;
using Heliopolis.Utilities;
using Heliopolis.Utilities.PathFinder;
using Heliopolis.Utilities.SpatialTreeIndexSystem;
using Heliopolis.World.ItemManagement;
using Heliopolis.World.State;
using Microsoft.Xna.Framework;

#endregion

namespace Heliopolis.World
{
    /// <summary>
    ///   Represents a single entity in the GameWorld that is an actual being.
    /// </summary>
    /// <remarks>
    ///   This entity has a few characteristics that mark it as being different from a static object.
    ///   The first is that it contains an inventory and can hold items. The second is that it has a complex
    ///   internal state, which is managed by the ActorState class.
    ///   The Actors are also able to fulfill designations.
    ///   Actors also retain an internal set of counters, for things like health, hunger, sleep and other needs.
    /// </remarks>
    [Serializable]
    public class Actor : TimedEventor, ICloneable, ICanHoldItem, ISpatialIndexMember
    {
        private Point _position;
        private MovementDestination<Point> _destinationPosition;
        private List<Item> _inventory;
        private int _hitpoints;
        private Dictionary<string, int> _properties;
        private readonly List<string> _jobsAble;
        private string _texture;
        private ActorState _state;
        private Dictionary<string, TimeSpan> _actionTimes;
        private string _actorType;

        private List<Item> _inHand;
        public List<Item> InHand
        {
            get { return _inHand; }
            set { _inHand = value; }
        }

        /// <summary>
        ///   The current section/area ID of this actor.
        /// </summary>
        public int AreaID
        {
            get { return Owner.Environment.GameWorld[_position.X, _position.Y].AreaID; }
        }

        /// <summary>
        ///   The type of this actor.
        /// </summary>
        public string ActorType
        {
            get { return _actorType; }
            set { _actorType = value; }
        }

        /// <summary>
        ///   Initialises a new instance of the Actor class.
        /// </summary>
        /// <param name = "owner">The owning GameWorld.</param>
        /// <param name = "actorType">The actor type.</param>
        /// <param name = "texture">The texture to draw.</param>
        /// <param name = "hitPoints">The starting number of hitpoints.</param>
        /// <param name = "properties">Any internal properties of this actor.</param>
        /// <param name = "jobsAble">The jobs this actor can initially perform.</param>
        public Actor(GameWorld owner,
                     string actorType,
                     string texture,
                     int hitPoints,
                     Dictionary<string, int> properties,
                     List<string> jobsAble) : base(owner)
        {
            this._texture = texture;
            _hitpoints = hitPoints;
            this._properties = properties;
            this._jobsAble = jobsAble;
            this._actorType = actorType;
            _position = new Point(-1, -1);
            TimedEventDisabled = true;
        }

        /// <summary>
        ///   Contains the various actions this actor can peform, and how long it takes to do each.
        /// </summary>
        public Dictionary<string, TimeSpan> ActionTimes
        {
            get { return _actionTimes; }
            set { _actionTimes = value; }
        }

        /// <summary>
        ///   If this actor is moving, this is a list of the directions to take to reach its destination.
        /// </summary>
        public LinkedList<Direction> Directions { get; private set; }

        /// <summary>
        ///   Contains the current Actor's state.
        /// </summary>
        public ActorState State
        {
            get { return _state; }
            set { ChangeState(value); }
        }

        /// <summary>
        ///   When set, this will make the actor determine a new path to the destination position.
        /// </summary>
        public MovementDestination<Point> DestinationPosition
        {
            set
            {
                _destinationPosition = value;
                MoveToPosition(value);
            }
            get { return _destinationPosition; }
        }

        /// <summary>
        ///   The texture of this actor to render.
        /// </summary>
        public string Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        /// <summary>
        ///   A list of jobs that this actor is able to perform.
        /// </summary>
        public List<string> JobsAble
        {
            get { return _jobsAble; }
        }

        /// <summary>
        ///   The number of hitpoints of this actor.
        /// </summary>
        public int Hitpoints
        {
            get { return _hitpoints; }
            set { _hitpoints = value; }
        }

        /// <summary>
        ///   A list of items this actor is currently carrying.
        /// </summary>
        public List<Item> Inventory
        {
            get { return _inventory; }
            set { _inventory = value; }
        }

        /// <summary>
        ///   The position of this Actor in the game world.
        /// </summary>
        public Point Position
        {
            get { return _position; }
            set { ChangePosition(value); }
        }

        /// <summary>
        ///   Moves this actor into a new state.
        /// </summary>
        /// <param name = "newState">The new state.</param>
        private void ChangeState(ActorState newState)
        {
            _state = newState;
            _state.OnEnter();
        }

        public void StoreItemInHand()
        {
            Item toStore = InHand[0];
            InHand.Remove(toStore);
            Inventory.Add(toStore);
            toStore.ItemState = ItemStates.InBackpack;
        }

        #region ICanHoldItem Members

        /// <summary>
        ///   Pick up an item and place into this actors inventory.
        /// </summary>
        /// <param name = "item">The item to pick up.</param>
        public ItemStates PickupItem(Item item)
        {
            InHand.Add(item);
            return ItemStates.BeingCarried;
        }

        /// <summary>
        ///   Place a certain item type on to another ICanHoldItem. **NOT IMPLEMENTED**
        /// </summary>
        /// <param name = "item">The item to place.</param>
        public void PutdownItem(Item item)
        {
            InHand.Remove(item);
        }

        #endregion

        /// <summary>
        ///   Pathfinding logic to get this actor to the specified position.
        /// </summary>
        /// <param name = "newPosition">The new position to move to.</param>
        private void MoveToPosition(MovementDestination<Point> newPosition)
        {
            PathFinder<Point> pathFinder = Global.PathFinder;
            if (newPosition.PointToMoveTo != _position)
            {
                PathfindRequest<Point> newrequest = null;
                switch (newPosition.MovementDestinationType)
                {
                    case MovementDestinationType.SinglePoint:
                        newrequest = new PathfindRequest<Point>(_position, newPosition.PointToMoveTo, this);
                        break;
                    case MovementDestinationType.MultiPoint:
                        newrequest = new PathfindRequest<Point>(_position, this, newPosition.PointsAcceptable);
                        break;
                    default:
                        throw new Exception("Can not move to an unaccessable position.");
                }
                pathFinder.NewSearch(newrequest);
                if (pathFinder.SearchStep(999) == SearchState.SearchStateSucceeded)
                {
                    PathfindAnswer theAnswer = pathFinder.FinalResult();
                    Directions = theAnswer.Directions;
                }
                else
                    throw new Exception("Unable to path to that position.");
            }
            else
            {
                Directions.Clear();
            }
        }

        /// <summary>
        ///   Check to see if this actor has moved into a new section/area.
        /// </summary>
        /// <param name = "newPosition">The new position to move to.</param>
        private void ChangePosition(Point newPosition)
        {
            if (Position != new Point(-1, -1))
                Owner.Environment[Position].ActorsOnTile.Remove(this);
            Owner.SpatialTreeIndex.CheckChangeSection(_position, newPosition, this, SpatialObjectType.Actor, "");
            _position = newPosition;
            Owner.Environment[Position].ActorsOnTile.Add(this);
        }

        /// <summary>
        ///   Move this actor in the next direction contained in it's internal Directions list.
        /// </summary>
        public void MoveNextDirection()
        {
            if (Directions.Count <= 0) return;
            Direction currentDirection = Directions.First.Value;
            switch (currentDirection)
            {
                case Direction.South:
                    ChangePosition(new Point(_position.X, _position.Y + 1));
                    break;
                case Direction.North:
                    ChangePosition(new Point(_position.X, _position.Y - 1));
                    break;
                case Direction.West:
                    ChangePosition(new Point(_position.X - 1, _position.Y));
                    break;
                case Direction.East:
                    ChangePosition(new Point(_position.X + 1, _position.Y));
                    break;
                case Direction.Nowhere:
                    break;
            }
            Directions.RemoveFirst();
        }

        /// <summary>
        ///   Process this actors move.
        /// </summary>
        /// <param name = "absoluteMilliseconds">The absolute game time.</param>
        public override void ExecuteTick(TimeSpan absoluteMilliseconds)
        {
            _state.Tick();
            if (_state.StateFinished)
            {
                _state = new ActorStateIdle(this, Owner);
            }
            // Want to set up the next tick
            SetUpNextTick(_actionTimes[_state.CurrentActionType]);
        }

        public void Start()
        {
            TimedEventDisabled = false;
        }

        /// <summary>
        ///   Creates a new copy of this class.
        /// </summary>
        /// <returns>An Actor copy.</returns>
        public object Clone()
        {
            Actor returnMe = (Actor) MemberwiseClone();
            returnMe.Inventory = new List<Item>();
            returnMe.ActionTimes = new Dictionary<string, TimeSpan>();
            returnMe.State = new ActorStateIdle(returnMe, Owner);
            returnMe.InHand = new List<Item>();
            return returnMe;
        }
    }
}