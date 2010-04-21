using System;
using Microsoft.Xna.Framework;
using Heliopolis.Utilities;

namespace Heliopolis.World
{
    /// <summary>
    /// Represents a collection of information pertaining to performance of jobs.
    /// </summary>
    /// <remarks>The job parameters are carried by designations, and given to actors when
    /// the designation is carried out. The job parameters tell the actor the actual job
    /// information so that the job can be carried out.</remarks>
    [Serializable]
    public abstract class JobParameters
    {
        private Actor jobActor;

        /// <summary>
        /// Initialises a new instance of the JobParameters class.
        /// </summary>
        public JobParameters()
        {

        }

        /// <summary>
        /// The actor who is performing this job.
        /// </summary>
        public Actor JobActor
        {
            get { return jobActor; }
            set { jobActor = value; }
        }

        // At the moment, the area ID is only being actually used with the building job parameter
        /// <summary>
        /// Returns pathing information so that the Actor can find its way to the job.
        /// </summary>
        /// <param name="areaId">The area ID of the searcher.</param>
        /// <returns>A MovementDestination object containing pathing destination information.</returns>
        public abstract MovementDestination<Point> GetJobAcccessPosition(int areaId);
        public abstract bool RequiresPositionalAccess();
    }

    [Serializable]
    public class EnvironmentalJobParameters : JobParameters
    {
        private EnvironmentTile targetTile;

        public EnvironmentalJobParameters(EnvironmentTile _targetTile)
            : base()
        {
            targetTile = _targetTile;
        }

        public EnvironmentTile TargetTile
        {
            get { return targetTile; }
        }

        public override MovementDestination<Point> GetJobAcccessPosition(int areaId)
        {
            return new MovementDestination<Point>(targetTile.Position);
        }

        public override bool RequiresPositionalAccess()
        {
            return true;
        }
    }

    [Serializable]
    public class BuildingJobParameters : JobParameters
    {
        private Building targetBuilding;

        public BuildingJobParameters(Building _targetBuilding)
            : base()
        {
            targetBuilding = _targetBuilding;
        }

        public Building TargetBuilding
        {
            get { return targetBuilding; }
        }

        public override MovementDestination<Point> GetJobAcccessPosition(int areaId)
        {
            return new MovementDestination<Point>(targetBuilding.Position, targetBuilding.ConstructionPoints(areaId));
        }

        public override bool RequiresPositionalAccess()
        {
            return true;
        }
    }

    [Serializable]
    public class MoveItemJobParameters : JobParameters
    {
        private string targetItemType;
        private Item targetItem = null;
        private ICanHoldItem targetHolder;

        public MoveItemJobParameters(String _targetItemType, ICanHoldItem _targetHolder)
            : base()
        {
            targetItemType = _targetItemType;
            targetHolder = _targetHolder;
        }

        public Item TargetItem
        {
            get { return targetItem; }
            set { targetItem = value; }
        }

        public ICanHoldItem TargetHolder
        {
            get { return targetHolder; }
        }

        public string TargetItemType
        {
            get { return targetItemType; }
        }

        public override MovementDestination<Point> GetJobAcccessPosition(int areaId)
        {
            if (targetItem != null)
                return new MovementDestination<Point>(targetItem.Position);
            else
                throw new Exception("Can not get the position of this job because no item has been specified.");
        }

        public override bool RequiresPositionalAccess()
        {
            return false;
        }
    }
}
