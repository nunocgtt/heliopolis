using System;
using System.Collections.Generic;
using System.Linq;
using Heliopolis.Utilities.PathFinder;
using Heliopolis.World.Environment;
using Heliopolis.World.InteractableObjects;
using Heliopolis.World.State;
using Microsoft.Xna.Framework;

namespace Heliopolis.World.JobSystem
{
    /// <summary>
    /// Designation to harvest a resource node.
    /// </summary>
    public class HarvestDesignation : Designation
    {
        public readonly HarvestableInteractableObject TargetToHarvest;

        public HarvestDesignation(GameWorld owner, HarvestableInteractableObject targetToHarvest)
            : base(owner, targetToHarvest, true)
        {
            TargetToHarvest = targetToHarvest;
            JobType = targetToHarvest.Action;
            IsReady = true;
        }

        public override List<ActorState> GetStateStepsToPerform()
        {
            var subStates =
                new List<ActorState>
                    {
                        new ActorStateMoveToICanAccess(TakenBy, TargetToHarvest, Owner),
                        new HarvestJob(Owner, TakenBy, JobType, this),
                        new ActorStateStashCurrentItem(TakenBy, Owner)
                    };
            return subStates;
        }

        public override bool Repeat()
        {
            return (TargetToHarvest.ResourceCount > 0);
        }
    }
}