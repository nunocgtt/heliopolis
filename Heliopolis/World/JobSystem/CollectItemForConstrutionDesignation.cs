using System;
using System.Collections.Generic;
using Heliopolis.World.BuildingManagement;
using Heliopolis.World.ItemManagement;
using Heliopolis.World.State;

namespace Heliopolis.World.JobSystem
{
    /// <summary>
    /// For when construction requires items, this will collect the items for it.
    /// </summary>
    public class CollectItemForConstrutionDesignation : Designation
    {
        public Item ItemToStash { get; set; }

        public CollectItemForConstrutionDesignation(GameWorld owner, string itemType, Building targetBuilding)
            : base(owner)
        {
            JobType = "MoveItem";
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

        public override bool Repeat()
        {
            throw new NotImplementedException();
        }
    }
}