using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Heliopolis.Utilities;

namespace Heliopolis.World
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
        /// Delegate type for checking if a state is finished.
        /// </summary>
        /// <returns></returns>
        protected delegate bool CheckFinishedState();
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
        protected CheckFinishedState checkFinishedState;

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
            if (checkFinishedState != null)
            {
                if (checkFinishedState())
                {
                    stateFinished = true;
                }
            }
        }
    }

    /// <summary>
    /// Represents an actor idle state where the actor looks for designations to complete.
    /// </summary>
    [Serializable]
    public class ActorStateIdle : ActorState
    {
        /// <summary>
        /// Initialises a new instance of the ActorStateIdle class.
        /// </summary>
        /// <param name="_myActor">The actor who this state belongs to.</param>
        /// <param name="_owner">The owning game world.</param>
        public ActorStateIdle(Actor _myActor, GameWorld _owner)
            : base(_myActor, _owner)
        {
            actionType = "idle";
            // Idle state is never finished. Instead it will move the actor into a valid state
            checkFinishedState = null;
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public override void Tick()
        {
            foreach (string s in myActor.JobsAble)
            {
                Designation someDesignation = owner.DesignationManager.CheckAvailableDesignation(myActor.AreaID, s, myActor.Position);
                if (someDesignation != null)
                {
                    someDesignation.AssignDesignation(myActor);
                    myActor.State = new ActorStateSatisfyDesignation(myActor, someDesignation, owner);
                    break;
                }
            }
            base.Tick();
        }
    }

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
            checkFinishedState = checkDesignationDone;
            subStates = new LinkedList<ActorState>();
            switch (myDesignation.DesignationType)
            {
                case DesignationTypes.Simple:
                    // TODO: Move the location chosing logic out of this class into the designation class
                    EnvironmentalJobParameters environmentalJobParameters = (EnvironmentalJobParameters)myDesignation.JobParameters;
                    MovementDestination<Point> movementDestination = _myDesignation.GetAccessablePointsByAreaID(myActor.AreaID);
                    AddSubState(new ActorStateMove(myActor, movementDestination, owner));
                    AddSubState(new ActorStatePerformJob(myActor, JobFactory.GetNewJob("mining", environmentalJobParameters), owner)); 
                    break;
                case DesignationTypes.Construction:
                    BuildingJobParameters buildingJobParameters = (BuildingJobParameters)myDesignation.JobParameters;
                    AddSubState(new ActorStateMove(myActor, buildingJobParameters.GetJobAcccessPosition(myActor.AreaID), owner));
                    AddSubState(new ActorStatePerformJob(myActor, JobFactory.GetNewJob("construction", buildingJobParameters), owner));
                    break;
                case DesignationTypes.TransportItem:
                    MoveItemJobParameters moveItemJobParameters = (MoveItemJobParameters)myDesignation.JobParameters;
                    // move to item, pick up item, move to target, put down item
                    AddSubState(new ActorStateMove(myActor, moveItemJobParameters.TargetItem.Position, owner));
                    AddSubState(new ActorStatePerformJob(myActor, JobFactory.GetNewJob("pickupitem", moveItemJobParameters), owner));
                    if (moveItemJobParameters.TargetHolder is ICanAccess)
                    {
                        ICanAccess canAccess = (ICanAccess)moveItemJobParameters.TargetHolder;
                        AddSubState(new ActorStateMove(myActor, canAccess.GetAccessablePointsByAreaID(myActor.AreaID, AccessReason.PlaceItem), owner));
                    }
                    else
                    //throw new Exception(
                    //    "Can not move an object to an object that does not implement the ICanAccess interface");
                    {
                        AddSubState(new ActorStateMove(myActor, moveItemJobParameters.TargetHolder.Position, owner));
                    }
                    AddSubState(new ActorStatePerformJob(myActor, JobFactory.GetNewJob("placeitem", moveItemJobParameters), owner)); 
                    break;
                default:
                    break;
            }
        }

        private bool checkDesignationDone()
        {
            // Designation is done when we have a.moved and b.performed the job
            // Todo: remove the designation because it has been completed
            if (subStates.Count == 0)
            {
                myDesignation.CompleteDesignation();
            }
            return (subStates.Count == 0);
        }
    }

    /// <summary>
    /// Represents an actor state for peforming a Job.
    /// </summary>
    /// <remarks>The actual peformance of the job is handled in the Job class Job.Tick()</remarks>
    [Serializable]
    public class ActorStatePerformJob : ActorState
    {
        private Job myJob;
        /// <summary>
        /// Initialises a new instance of the ActorStatePerformJob class.
        /// </summary>
        /// <param name="_myActor">The actor who this state belongs to.</param>
        /// <param name="_myJob">The job to perform.</param>
        /// <param name="_owner">The owning game world.</param>
        public ActorStatePerformJob(Actor _myActor, Job _myJob, GameWorld _owner)
            : base(_myActor, _owner)
        {
            actionType = _myJob.JobType;
            myJob = _myJob;
            checkFinishedState = checkJobDone;
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public override void Tick()
        {
            myJob.Tick();
            base.Tick();
        }

        private bool checkJobDone()
        {
            return (myJob.IsFinished);
        }
    }

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
            checkFinishedState = checkMoveDone;
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
            checkFinishedState = checkMoveDone;
            movementDestination = _movementDestination;
        }

        /// <summary>
        /// Set the actor's destination when this state is entered.
        /// </summary>
        public override void OnEnter()
        {
            myActor.DestinationPosition = movementDestination;
            base.OnEnter();
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public override void Tick()
        {
            myActor.moveNextDirection();
            base.Tick();
        }

        private bool checkMoveDone()
        {
            return (myActor.Directions.Count == 0);
        }
    }
}
