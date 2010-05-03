using System;
using System.Collections.Generic;
using System.Linq;
using Heliopolis.Utilities.PathFinder;
using Heliopolis.World.Environment;
using Heliopolis.World.ItemManagement;
using Microsoft.Xna.Framework;
using Heliopolis.Utilities;
using Heliopolis.World.State;

namespace Heliopolis.World.JobSystem
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
        /// <param name="owner">The owning game world.</param>
        protected Designation(GameWorld owner)
            : base(owner)
        {
            _isTaken = false;
            IsComplete = false;
            Owner = owner;
            JobType = "";
        }

        public string JobType { get; set; }
        private bool _isTaken = false;

        public bool IsTaken
        {
            get
            {
                return _isTaken;
            }
            set
            {
                _isTaken = value;
                UpdateIsAvailableForTaking();
            }
        }

        public bool IsComplete { get; set; }
        public Designation PostRequisite { get; set; }
        public Actor TakenBy { get; set; }

        private bool _isAvailableForTakingOldValue = false;
        private List<EnvironmentTile> _accessPoints = new List<EnvironmentTile>();
        private readonly List<EnvironmentTile> _accessiblePoints = new List<EnvironmentTile>();

        public List<EnvironmentTile>  AccessPoints 
        {
            get
            {
                return _accessPoints;
            }
            set
            {
                if (_accessPoints.Count > 0)
                {
                    foreach (EnvironmentTile tile in _accessPoints)
                    {
                        tile.RequiringAccess.Remove(this);
                    }
                }
                _accessiblePoints.Clear();
                _accessPoints = value;
                foreach (EnvironmentTile tile in _accessPoints)
                {
                    tile.RequiringAccess.Add(this);
                    if (tile.CanAccess)
                        _accessiblePoints.Add(tile);
                }
                UpdateIsAvailableForTaking();
            }
        }

        public bool CanBeTakenFromArea(int areaId)
        {
            return _accessiblePoints.Any(tile => tile.AreaID == areaId);
        }

        public MovementDestination<Point> GetAccessablePointsByAreaID(int areaId)
        {
            MovementDestination<Point> returnMe = new MovementDestination<Point>();
            foreach (EnvironmentTile tile in _accessiblePoints)
                if (tile.AreaID == areaId)
                    returnMe.PointsAcceptable.Add(tile.Position);
            return returnMe;
        }

        public bool CanAccess
        {
            get
            {
                return _accessiblePoints.Count > 0;
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
        /// <returns>Returns true is the designation can be taken.</returns>
        public bool HasPrerequisites
        {
            get
            {
                return prerequisites.Count > 0;
            }
        }

        protected void AddPrerequisite(Designation preReq)
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
            UpdateIsAvailableForTaking();
        }

        /// <summary>
        /// Complete this designation.
        /// </summary>
        public void CompleteDesignation()
        {
            IsComplete = true;
            if (PostRequisite != null)
                PostRequisite.RemovePrerequisite(this);
            Owner.DesignationManager.DesignationCompleted(this);
        }

        public bool IsAvailableToTake
        {
            get
            {
                return CanAccess && !HasPrerequisites && !IsTaken;
            }
        }

        private void UpdateIsAvailableForTaking()
        {
            bool canNowTake = IsAvailableToTake;
            if (!_isAvailableForTakingOldValue && canNowTake)
            {
                Owner.DesignationManager.MakeDesignationAvailable(this);
            }
            else if (_isAvailableForTakingOldValue && !canNowTake)
            {
                Owner.DesignationManager.MakeDesignationUnavailable(this);
            }
            _isAvailableForTakingOldValue = canNowTake;
        }

        #region IRequiresAccess Members

        public void AccessChanged(bool canAccess, Point position)
        {
            if (canAccess)
                _accessiblePoints.Add(Owner.Environment[position]);
            else
                _accessiblePoints.Remove(Owner.Environment[position]);
            UpdateIsAvailableForTaking();
        }

        #endregion

        public abstract List<ActorState> GetStateStepsToPerform();
    }

    /// <summary>
    /// Designation to harvest a resource node. TODO: Ensure this can be repeated by re-creating it after it's done.
    /// </summary>
    public class HarvestDesignation : Designation
    {
        public HarvestDesignation(GameWorld owner, EnvironmentTile targetTile, string jobtype)
            : base(owner)
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
            subStates.Add(new ActorStateMove(TakenBy, movementDestination, Owner));
            subStates.Add(new HarvestJob(Owner, TakenBy, JobType, this));
            return subStates;
        }
    }

    /// <summary>
    /// For when an item is dropped on the ground, it will eventually need to be put somewhere nicer.
    /// </summary>
    public class StashItemFromGroundDesignation : Designation
    {
        public Item ItemToStash { get; set; }

        public StashItemFromGroundDesignation(GameWorld owner, Item itemToStash, string jobtype)
            : base(owner)
        {
            this.ItemToStash = itemToStash;
            this.JobType = "MoveItem";
            this.AccessPoints = new List<EnvironmentTile>() { Owner.Environment[ItemToStash.Position] };
        }

        public override List<ActorState> GetStateStepsToPerform()
        {
            List<ActorState> subStates = new List<ActorState>();
            subStates.Add(new ActorStateMove(TakenBy, ItemToStash.Position, Owner));
            // pick up item
            // find a stash to take it to
            // move to that stash
            return subStates;
        }
    }

    public class CollectItemDesignation : Designation
    {
        public Item ItemToStash { get; set; }

        public CollectItemDesignation(GameWorld owner, string itemType)
            : base(owner)
        {
            this.JobType = "MoveItem";
            // TODO: Find an item first!!!
            //this.AccessPoints = new List<EnvironmentTile>() { owner.Environment[ItemToStash.Position] };
        }

        public override List<ActorState> GetStateStepsToPerform()
        {
            List<ActorState> subStates = new List<ActorState> {new ActorStateMove(TakenBy, ItemToStash.Position, Owner)};
            return subStates;
        }
    }

    /// <summary>
    /// For when construction requires items, this will collect the items for it.
    /// </summary>
    public class CollectItemForConstrutionDesignation : Designation
    {
        public Item ItemToStash { get; set; }

        public CollectItemForConstrutionDesignation(GameWorld _owner, string itemType, Building targetBuilding)
            : base(_owner)
        {
            this.JobType = "MoveItem";
            // TODO: Find an item first!!!
            //this.AccessPoints = new List<EnvironmentTile>() { owner.Environment[ItemToStash.Position] };
        }

        public override List<ActorState> GetStateStepsToPerform()
        {
            List<ActorState> subStates = new List<ActorState>();
            subStates.Add(new ActorStateMove(TakenBy, ItemToStash.Position, Owner));
            // Pick up item
            // take item to buiding
            // place item in building
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
