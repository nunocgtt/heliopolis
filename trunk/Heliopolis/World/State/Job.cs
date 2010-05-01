using System;
using System.Collections.Generic;
using System.Text;
using Heliopolis.World.JobSystem;
using Microsoft.Xna.Framework;
using System.Xml;
using Heliopolis.World.State;

namespace Heliopolis.World
{
    /// <summary>
    /// Represents a single job that an actor can perform.
    /// </summary>
    /// <remarks>Jobs are performed from within ActorStatePerformJob instances. Of importance is the
    /// JobParameter, which contains the various relevant information that the actor needs to actually
    /// get to and perform the job.</remarks>
    [Serializable]
    public abstract class Job : ActorState
    {
        private string jobType;
        private bool isFinished;

        /// <summary>
        /// If the current job is finished.
        /// </summary>
        public bool IsFinished
        {
            get { return isFinished; }
            set { isFinished = value; }
        }

        /// <summary>
        /// The type of this job.
        /// </summary>
        public string JobType
        {
            get { return jobType; }
            set { jobType = value; }
        }

        public Designation OwningDesignation { get; set; }

        /// <summary>
        /// Initialises a new instance of the Job class.
        /// </summary>
        /// <param name="_owner">The owning game world.</param>
        /// <param name="_jobtype">The type of this job.</param>
        public Job(GameWorld _owner, Actor _actor, string _jobtype, Designation owningDesignation)
            : base(_actor,_owner)
        {
            jobType = _jobtype;
            isFinished = false;
            OwningDesignation = owningDesignation;
        }

        //{
        //    isFinished = true;
        //    switch (jobType)
        //    {
        //        case "mining":
        //            EnvironmentalJobParameters environmentalJobParameters = (EnvironmentalJobParameters)JobParameters;
        //            environmentalJobParameters.TargetTile.ResourceLeft -= 1;
        //            owner.ItemManager.SpawnItem(environmentalJobParameters.TargetTile.Resource, environmentalJobParameters.JobActor.Position);
        //            // we need to create some resource where the actor is
        //            isFinished = false;
        //            if (environmentalJobParameters.TargetTile.ResourceLeft == 0)
        //            {
        //                EnvironmentTileFactory.SetToTemplate(environmentalJobParameters.TargetTile.ExhaustedTile, environmentalJobParameters.TargetTile);
        //                isFinished = true;
        //            }
        //            break;
        //        case "construction":
        //            BuildingJobParameters buildingJobParameters = (BuildingJobParameters)jobParameters;
        //            buildingJobParameters.TargetBuilding.CompleteBuilding();
        //            break;
        //    }
        //}

        /// <summary>
        /// Creates a copy of this Job.
        /// </summary>
        /// <returns>A Job copy.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class HarvestJob : Job
    {
        public HarvestJob(GameWorld _owner, Actor _actor, string _jobType, HarvestDesignation parentDesignation)
            : base(_owner, _actor, _jobType, parentDesignation)
        {

        }

        public override void Tick()
        {
            throw new NotImplementedException();
        }

        protected override bool CheckFinishedState()
        {
            return this.IsFinished;
        }
    }

    public class PickupItemJob : Job
    {
        public ICanHoldItem PickerUpper { get; set; }
        public Item TargetItem { get; set; }

        public PickupItemJob(GameWorld _owner, Actor _actor, string _jobType, Designation parentDesignation, ICanHoldItem pickUpper, Item targetItem)
            : base(_owner, _actor, _jobType, parentDesignation)
        {
            PickerUpper = pickUpper;
        }

        public override void Tick()
        {
            PickerUpper.PickupItem(TargetItem);
        }

        protected override bool CheckFinishedState()
        {
            return this.IsFinished;
        }
    }

    public class PlaceItem : Job
    {
        public ICanHoldItem ActorPlacingItem { get; set; }
        public ICanHoldItem PutItemPlace { get; set; }
        public Item TargetItem { get; set; }

        public PlaceItem(GameWorld _owner, Actor _actor, string _jobType, Designation parentDesignation, ICanHoldItem putItemPlace, Item targetItem)
            : base(_owner, _actor, _jobType, parentDesignation)
        {
            PutItemPlace = putItemPlace;
            ActorPlacingItem = _actor;
        }

        public override void Tick()
        {
            ActorPlacingItem.PlaceItem(PutItemPlace, TargetItem);
        }

        protected override bool CheckFinishedState()
        {
            return this.IsFinished;
        }
    }
}
