using System;
using System.Collections.Generic;
using Heliopolis.World.Environment;
using Heliopolis.World.ItemManagement;
using Heliopolis.World.State;

namespace Heliopolis.World.JobSystem
{
    /// <summary>
    /// For when an item is dropped on the ground, it will eventually need to be put somewhere nicer.
    /// </summary>
    public class StashItemFromGroundDesignation : Designation
    {
        public Item ItemToStash { get; set; }

        public StashItemFromGroundDesignation(GameWorld owner, Item itemToStash, string jobtype)
            : base(owner)
        {
            ItemToStash = itemToStash;
            JobType = "MoveItem";
            AccessPoints = new List<EnvironmentTile> { Owner.Environment[ItemToStash.Position] };
        }

        public override List<ActorState> GetStateStepsToPerform()
        {
            var subStates =
                new List<ActorState>
                    {
                        new ActorStateMove(TakenBy, ItemToStash.Position, Owner),
                        new ActorStatePickupItem(TakenBy, ItemToStash, Owner),
                        new ActorStateStashCurrentItem(TakenBy, Owner)
                    };
            return subStates;
        }

        public override bool Repeat()
        {
            throw new NotImplementedException();
        }
    }
}