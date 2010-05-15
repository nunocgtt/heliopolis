using System;
using Heliopolis.World.ItemManagement;
using Heliopolis.World.JobSystem;

namespace Heliopolis.World.State
{
    public class HarvestJob : Job
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HarvestJob"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="actor">The actor.</param>
        /// <param name="jobType">Type of the job.</param>
        /// <param name="parentDesignation">The parent designation.</param>
        public HarvestJob(GameWorld owner, Actor actor, string jobType, HarvestDesignation parentDesignation)
            : base(owner, actor, jobType, parentDesignation)
        {
            ActionType = "harvest";
        }

        public override void OnEnter()
        {
            
        }

        public override void OnFinish()
        {
            
        }

        public override void Tick()
        {
            Item harvestedItem;
            (OwningDesignation as HarvestDesignation).TargetToHarvest.Harvest(out harvestedItem);
            ItemManager.OnItemCreate(MyActor, harvestedItem);
            Finished = true;
        }

    }
}