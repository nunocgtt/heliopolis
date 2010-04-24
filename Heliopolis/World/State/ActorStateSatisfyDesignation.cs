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
            checkFinishedState = checkDesignationDone;
            subStates = new LinkedList<ActorState>();
            //switch (myDesignation.DesignationType)
            //{
            //    case DesignationTypes.Simple:
            //        // TODO: Move the location chosing logic out of this class into the designation class
            //        EnvironmentalJobParameters environmentalJobParameters = (EnvironmentalJobParameters)myDesignation.JobParameters;
            //        MovementDestination<Point> movementDestination = _myDesignation.GetAccessablePointsByAreaID(myActor.AreaID);
            //        AddSubState(new ActorStateMove(myActor, movementDestination, owner));
            //        AddSubState(new ActorStatePerformJob(myActor, JobFactory.GetNewJob("mining", environmentalJobParameters), owner));
            //        break;
            //    case DesignationTypes.Construction:
            //        BuildingJobParameters buildingJobParameters = (BuildingJobParameters)myDesignation.JobParameters;
            //        AddSubState(new ActorStateMove(myActor, buildingJobParameters.GetJobAcccessPosition(myActor.AreaID), owner));
            //        AddSubState(new ActorStatePerformJob(myActor, JobFactory.GetNewJob("construction", buildingJobParameters), owner));
            //        break;
            //    case DesignationTypes.TransportItem:
            //        MoveItemJobParameters moveItemJobParameters = (MoveItemJobParameters)myDesignation.JobParameters;
            //        // move to item, pick up item, move to target, put down item
            //        AddSubState(new ActorStateMove(myActor, moveItemJobParameters.TargetItem.Position, owner));
            //        AddSubState(new ActorStatePerformJob(myActor, JobFactory.GetNewJob("pickupitem", moveItemJobParameters), owner));
            //        if (moveItemJobParameters.TargetHolder is ICanAccess)
            //        {
            //            ICanAccess canAccess = (ICanAccess)moveItemJobParameters.TargetHolder;
            //            AddSubState(new ActorStateMove(myActor, canAccess.GetAccessablePointsByAreaID(myActor.AreaID, AccessReason.PlaceItem), owner));
            //        }
            //        else
            //        //throw new Exception(
            //        //    "Can not move an object to an object that does not implement the ICanAccess interface");
            //        {
            //            AddSubState(new ActorStateMove(myActor, moveItemJobParameters.TargetHolder.Position, owner));
            //        }
            //        AddSubState(new ActorStatePerformJob(myActor, JobFactory.GetNewJob("placeitem", moveItemJobParameters), owner));
            //        break;
            //    default:
            //        break;
            //}
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
}
