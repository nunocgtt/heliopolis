using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Heliopolis.Utilities;
using Heliopolis.World.State;

namespace Heliopolis.World
{
    /// <summary>
    /// Represents a single designation in the game world.
    /// </summary>
    /// <remarks>A designation is a user requirement for change in the game world. Through the user interface,
    /// the user is able to specify they want something done. This action is then stored as a designation. Actors
    /// then are able to take up designations and complete them at their leasure.
    /// Designations can have prerequisites. These need to be completed before it can be picked up.</remarks>
    [Serializable]
    public abstract class Designation : GameWorldObject, IRequiresAccess
    {
        /// <summary>
        /// Initialises a new instance of the Designation class.
        /// </summary>
        /// <param name="_owner">The owning game world.</param>
        /// <param name="_jobParameters">Any relevant job parameters for this designation.</param>
        /// <param name="_designationType">The type of designation.</param>
        public Designation(GameWorld _owner)
            : base(_owner)
        {
            isTaken = false;
            IsComplete = false;
            owner = _owner;
            JobType = "";
        }

        public string JobType { get; set; }
        private bool isTaken = false;

        public bool IsTaken
        {
            get
            {
                return isTaken;
            }
            set
            {
                isTaken = value;
                updateIsAvailableForTaking();
            }
        }

        public bool IsComplete { get; set; }
        public Designation PostRequisite { get; set; }
        public Actor TakenBy { get; set; }

        private bool isAvailableForTakingOldValue = false;
        private List<EnvironmentTile> accessPoints = new List<EnvironmentTile>();
        private List<EnvironmentTile> accessiblePoints = new List<EnvironmentTile>();

        public List<EnvironmentTile>  AccessPoints 
        {
            get
            {
                return accessPoints;
            }
            set
            {
                if (accessPoints.Count > 0)
                {
                    foreach (EnvironmentTile tile in accessPoints)
                    {
                        tile.RequiringAccess.Remove(this);
                    }
                }
                accessiblePoints.Clear();
                accessPoints = value;
                foreach (EnvironmentTile tile in accessPoints)
                {
                    tile.RequiringAccess.Add(this);
                    if (tile.CanAccess)
                        accessiblePoints.Add(tile);
                }
                updateIsAvailableForTaking();
            }
        }

        public bool CanBeTakenFromArea(int areaId)
        {
            foreach (EnvironmentTile tile in accessiblePoints)
                if (tile.AreaID == areaId)
                    return true;
            return false;
        }

        public MovementDestination<Point> GetAccessablePointsByAreaID(int areaId)
        {
            MovementDestination<Point> returnMe = new MovementDestination<Point>();
            foreach (EnvironmentTile tile in accessiblePoints)
                if (tile.AreaID == areaId)
                    returnMe.PointsAcceptable.Add(tile.Position);
            return returnMe;
        }

        public bool CanAccess
        {
            get
            {
                return accessiblePoints.Count > 0;
            }
        }

        protected List<Designation> prerequisites = new List<Designation>();

        /// <summary>
        /// A list of designations that are prerequisites.
        /// </summary>
        public List<Designation> Prerequisites
        {
            get { return prerequisites; }
            set { prerequisites = value; }
        }

        /// <summary>
        /// Assigns a designation to an actor that is able to carry it out.
        /// </summary>
        /// <param name="myActor">The Actor to be assigned the designation.</param>
        public void AssignDesignation(Actor myActor)
        {
            IsTaken = true;
            TakenBy = myActor;
        }

        /// <summary>
        /// Unassigns a designation.
        /// </summary>
        public void UnassignDesignation()
        {
            IsTaken = false;
            TakenBy = null;
        }
        
        /// <summary>
        /// Checks to see if a designation can be taken by an actor in a specific area ID. Makes a few checks
        /// depending on the state of this designation, such as if there are prerequisites, locational area checks
        /// and if there are any items to satisfy this designation.
        /// </summary>
        /// <param name="searcherAreaId">The ID of the searcher.</param>
        /// <returns>Returns true is the designation can be taken.</returns>
        public bool HasPrerequisites
        {
            get
            {
                return prerequisites.Count > 0;
            }
        }

        protected void addPrerequisite(Designation preReq)
        {
            prerequisites.Add(preReq);
            preReq.PostRequisite = this;
        }

        /// <summary>
        /// Removes a prerequisite from this designation.
        /// </summary>
        /// <param name="preReq">The prerequisite Designation to remove.</param>
        public void RemovePrerequisite(Designation preReq)
        {
            prerequisites.Remove(preReq);
            updateIsAvailableForTaking();
        }

        /// <summary>
        /// Complete this designation.
        /// </summary>
        public void CompleteDesignation()
        {
            IsComplete = true;
            if (PostRequisite != null)
                PostRequisite.RemovePrerequisite(this);
            owner.DesignationManager.DesignationCompleted(this);
        }

        public bool IsAvailableToTake
        {
            get
            {
                return CanAccess && !HasPrerequisites && !IsTaken;
            }
        }

        private void updateIsAvailableForTaking()
        {
            bool canNowTake = IsAvailableToTake;
            if (!isAvailableForTakingOldValue && canNowTake)
            {
                owner.DesignationManager.MakeDesignationAvailable(this);
            }
            else if (isAvailableForTakingOldValue && !canNowTake)
            {
                owner.DesignationManager.MakeDesignationUnavailable(this);
            }
            isAvailableForTakingOldValue = canNowTake;
        }

        #region IRequiresAccess Members

        public void AccessChanged(bool canAccess, Point position)
        {
            if (canAccess)
                accessiblePoints.Add(owner.Environment[position]);
            else
                accessiblePoints.Remove(owner.Environment[position]);
            updateIsAvailableForTaking();
        }

        #endregion

        public abstract List<ActorState> GetStateStepsToPerform();
    }


    public class HarvestDesignation : Designation
    {
        public HarvestDesignation(GameWorld _owner, EnvironmentTile targetTile, string jobtype)
            : base(_owner)
        {
            this.JobType = jobtype;
            List<EnvironmentTile> access = new List<EnvironmentTile>();
            foreach (EnvironmentTile tile in targetTile.AdjacentTiles)
                access.Add(tile);
            this.AccessPoints = access;
        }

        public override List<ActorState> GetStateStepsToPerform()
        {
            List<ActorState> subStates = new List<ActorState>();
            MovementDestination<Point> movementDestination = this.GetAccessablePointsByAreaID(TakenBy.AreaID);
            subStates.Add(new ActorStateMove(TakenBy, movementDestination, owner));

            return subStates;
        }
    }

    
    //public class BuildingCostructionDesignation : Designation
    //{
    //    public BuildingCostructionDesignation(GameWorld _owner, string _jobType, JobParameters _jobParameters, DesignationTypes _designationType,
    //        Point targetPos, string jobType, string buildingToConstruct) : base(_owner,  _jobType,  _jobParameters,  _designationType)
    //    {
    //        Building constructMe = owner.BuildingManager.StartBuildingConstruction(buildingToConstruct, targetPos);
    //        BuildingJobParameters buildingJobParameters = new BuildingJobParameters(constructMe);
    //        Designation newDesignation = new Designation(owner, jobType, buildingJobParameters, DesignationTypes.Construction);

    //        int j = 0;
    //        foreach (string item in BuildingFactory.BuildingTemplates[buildingToConstruct].RequiredMaterials)
    //        {
    //            for (int i = 0; i < BuildingFactory.BuildingTemplates[buildingToConstruct].RequiredMaterialAmount[j]; i++)
    //            {
    //                MoveItemJobParameters moveItemJobParameters = new MoveItemJobParameters(item, constructMe);
    //                Designation pickupItemDesignation = new Designation(owner, "moveitem", moveItemJobParameters, DesignationTypes.TransportItem);
    //                newDesignation.AddPrerequisite(pickupItemDesignation);
    //                //addDesignation("moveitem", pickupItemDesignation);
    //            }
    //            j++;
    //        }
    //    }
    //}



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
