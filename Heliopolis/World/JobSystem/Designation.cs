using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Heliopolis.Utilities;

namespace Heliopolis.World
{
    /// <summary>
    /// The various types of designations.
    /// </summary>
    public enum DesignationTypes
    {
        /// <summary>
        /// Simple designation is like harvesting. Move, then perform job.
        /// </summary>
        Simple,
        /// <summary>
        /// For the construction of buildings.
        /// </summary>
        Construction,
        /// <summary>
        /// For the transporting of items.
        /// </summary>
        TransportItem
    }
    /// <summary>
    /// Represents a single designation in the game world.
    /// </summary>
    /// <remarks>A designation is a user requirement for change in the game world. Through the user interface,
    /// the user is able to specify they want something done. This action is then stored as a designation. Actors
    /// then are able to take up designations and complete them at their leasure.
    /// Designations can have prerequisites. These need to be completed before it can be picked up.</remarks>
    [Serializable]
    public class Designation : GameWorldObject
    {
        protected JobParameters jobParameters;
        protected DesignationTypes designationType;
        protected bool physicallyAccessible;
        protected string jobType;
        protected List<Designation> prerequisites = null;
        protected bool isTaken;
        protected bool isComplete;
        protected Designation postRequisite;

        /// <summary>
        /// If this designation has already been taken by an actor.
        /// </summary>
        public bool IsTaken
        {
            get { return isTaken; }
        }

        /// <summary>
        /// A list of designations that are prerequisites.
        /// </summary>
        public List<Designation> Prerequisites
        {
            get { return prerequisites; }
            set { prerequisites = value; }
        }

        /// <summary>
        /// If this designation has been completed.
        /// </summary>
        public bool IsComplete
        {
            get { return isComplete; }
        }

        /// <summary>
        /// Why type of designation this is.
        /// </summary>
        public DesignationTypes DesignationType
        {
            get { return designationType; }
        }

        /// <summary>
        /// Any relevant job parameters that pertain to this designation.
        /// </summary>
        public JobParameters JobParameters
        {
            get { return jobParameters; }
        }

        /// <summary>
        /// If this designation is a prerequisite, this is a designation that owns it.
        /// </summary>
        public Designation PostRequisite
        {
            get { return postRequisite; }
            set { postRequisite = value; }
        }

        /// <summary>
        /// Initialises a new instance of the Designation class.
        /// </summary>
        /// <param name="_owner">The owning game world.</param>
        /// <param name="_jobParameters">Any relevant job parameters for this designation.</param>
        /// <param name="_designationType">The type of designation.</param>
        public Designation(GameWorld _owner, JobParameters _jobParameters, DesignationTypes _designationType) : base(_owner)
        {
            physicallyAccessible = false;
            isTaken = false;
            isComplete = false;
            owner = _owner;
            jobParameters = _jobParameters;
            designationType = _designationType;
        }

        /// <summary>
        /// Assigns a designation to an actor that is able to carry it out.
        /// </summary>
        /// <param name="myActor">The Actor to be assigned the designation.</param>
        public void AssignDesignation(Actor myActor)
        {
            jobParameters.JobActor = myActor;
            isTaken = true;
            if (jobParameters is MoveItemJobParameters)
            {
                MoveItemJobParameters moveItemJobParameters = (MoveItemJobParameters) jobParameters;
                moveItemJobParameters.TargetItem =
                    owner.ItemManager.GetClosestItem(moveItemJobParameters.JobActor.AreaID, moveItemJobParameters.TargetItemType, myActor.Position);
                moveItemJobParameters.TargetItem.IsReserved = true;
            }
        }

        /// <summary>
        /// Unassigns a designation.
        /// </summary>
        public void UnassignDesignation()
        {
            isTaken = false;
            //designationJob = null;
            throw new Exception("Not implemented yet");
        }
        
        /// <summary>
        /// Checks to see if a designation can be taken by an actor in a specific area ID. Makes a few checks
        /// depending on the state of this designation, such as if there are prerequisites, locational area checks
        /// and if there are any items to satisfy this designation.
        /// </summary>
        /// <param name="searcherAreaId">The ID of the searcher.</param>
        /// <returns>Returns true is the designation can be taken.</returns>
        public bool CanBeTaken(int searcherAreaId, Point searcherPosition)
        {
            if (prerequisites != null)
                if (prerequisites.Count > 0)
                    return false;
            if (!isTaken)
            {
                if (jobParameters.RequiresPositionalAccess())
                {
                    if (accessableFromAreaID(searcherAreaId))
                        return true;                
                }
                else 
                {
                    if (jobParameters is MoveItemJobParameters)
                    {
                        MoveItemJobParameters moveItemJobParameters = (MoveItemJobParameters)jobParameters;
                        return (owner.ItemManager.ValidItemExists(searcherAreaId, moveItemJobParameters.TargetItemType));
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a prerequisite designation on to this designation.
        /// </summary>
        /// <param name="preReq">The Designation that is a prerequisite.</param>
        public void AddPrerequisite(Designation preReq)
        {
            if (prerequisites == null)
            {
                prerequisites = new List<Designation>();
            }
            prerequisites.Add(preReq);
            preReq.PostRequisite = this;
        }

        /// <summary>
        /// Removes a prerequisite from this designation.
        /// </summary>
        /// <param name="preReq">The prerequisite Designation to remove.</param>
        public void RemovePrerequisite(Designation preReq)
        {
            if (prerequisites != null)
            {
                prerequisites.Remove(preReq);
            }
        }

        /// <summary>
        /// Checks to see if this designation can be accessed from a specific area.
        /// </summary>
        /// <param name="areaID">The area ID of the searching object.</param>
        /// <returns>Returns true if the searcher has access.</returns>
        private bool accessableFromAreaID(int areaID)
        {
            EnvironmentTile targetTile = owner.Environment[jobParameters.GetJobAcccessPosition(areaID).PointToMoveTo];
            if (targetTile.CanAccess)
            {
                return (targetTile.AreaID == areaID);
            }
            else
            {
                List<EnvironmentTile> adjacent = targetTile.AdjacentTiles;
                foreach (EnvironmentTile tile in adjacent)
                {
                    if (tile != null)
                        if (tile.AreaID == areaID)
                            return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns pathing information to access this designation from a particular area ID.
        /// </summary>
        /// <param name="areaID">The area ID of the searcher.</param>
        /// <returns>A MovementDestination object containing relevant pathing destination information.</returns>
        public MovementDestination<Point> GetAccessablePointsByAreaID(int areaID)
        {
            EnvironmentTile targetTile = owner.Environment[jobParameters.GetJobAcccessPosition(areaID).PointToMoveTo];
            if (targetTile.CanAccess)
            {
                if (targetTile.AreaID == areaID)
                    return new MovementDestination<Point>(targetTile.Position);
                else
                    return new MovementDestination<Point>();
            }
            else
            {
                List<Point> returnme = new List<Point>();
                List<EnvironmentTile> adjacent = targetTile.AdjacentTiles;
                foreach (EnvironmentTile tile in adjacent)
                {
                    if (tile != null)
                        if (tile.AreaID == areaID)
                            returnme.Add(tile.Position);
                }
                return new MovementDestination<Point>(targetTile.Position, returnme);
            }
        }

        /// <summary>
        /// Complete this designation.
        /// </summary>
        public void CompleteDesignation()
        {
            isComplete = true;
            if (postRequisite != null)
                postRequisite.RemovePrerequisite(this);
        }

    }

    //public class HarvestDesignation : Designation
    //{
    //    public HarvestDesignation(GameWorld _owner, string _jobType, EnvironmentTile targetTile)
    //        : base(_owner, _jobType)
    //    {
    //        jobParameters = new EnvironmentalJobParameters(targetTile);
    //        designationType = DesignationTypes.Simple;
    //        owner.DesignationManager.AddDesignation(jobType, this);
    //    }
    //}

    
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
}
