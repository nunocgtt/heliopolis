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
        private readonly HarvestableInteractableObject _targetToHarvest;

        public HarvestDesignation(GameWorld owner, HarvestableInteractableObject targetToHarvest, string jobtype)
            : base(owner, targetToHarvest, true)
        {
            _targetToHarvest = targetToHarvest;
            JobType = jobtype;
        }

        public override List<ActorState> GetStateStepsToPerform()
        {
            List<ActorState> subStates =
                new List<ActorState>
                    {
                        new ActorStateMoveToICanAccess(TakenBy, _targetToHarvest, Owner),
                        new HarvestJob(Owner, TakenBy, JobType, this)
                    };
            return subStates;
        }

        public override bool Repeat()
        {
            return (_targetToHarvest.ResourceCount > 0);
        }
    }
}