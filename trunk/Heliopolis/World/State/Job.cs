using System;
using Heliopolis.World.JobSystem;

namespace Heliopolis.World.State
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
        private string _jobType;
        private bool _isFinished;

        /// <summary>
        /// If the current job is finished.
        /// </summary>
        public bool IsFinished
        {
            get { return _isFinished; }
            set { _isFinished = value; }
        }

        /// <summary>
        /// The type of this job.
        /// </summary>
        public string JobType
        {
            get { return _jobType; }
            set { _jobType = value; }
        }

        public Designation OwningDesignation { get; set; }

        /// <summary>
        /// Initialises a new instance of the Job class.
        /// </summary>
        /// <param name="owner">The owning game world.</param>
        /// <param name="actor"></param>
        /// <param name="jobtype">The type of this job.</param>
        /// <param name="owningDesignation"></param>
        protected Job(GameWorld owner, Actor actor, string jobtype, Designation owningDesignation)
            : base(actor,owner)
        {
            _jobType = jobtype;
            _isFinished = false;
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
        public HarvestJob(GameWorld owner, Actor actor, string jobType, HarvestDesignation parentDesignation)
            : base(owner, actor, jobType, parentDesignation)
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

        public PickupItemJob(GameWorld owner, Actor actor, string jobType, Designation parentDesignation, ICanHoldItem pickUpper, Item targetItem)
            : base(owner, actor, jobType, parentDesignation)
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

        public PlaceItem(GameWorld owner, Actor actor, string jobType, Designation parentDesignation, ICanHoldItem putItemPlace, Item targetItem)
            : base(owner, actor, jobType, parentDesignation)
        {
            PutItemPlace = putItemPlace;
            ActorPlacingItem = actor;
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
