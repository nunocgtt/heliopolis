using System;
using Heliopolis.World.ItemManagement;
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

        /// <summary>
        /// Gets or sets the owning designation.
        /// </summary>
        /// <value>The owning designation.</value>
        public Designation OwningDesignation { get; set; }

        /// <summary>
        /// Initialises a new instance of the Job class.
        /// </summary>
        /// <param name="owner">The owning game world.</param>
        /// <param name="actor"></param>
        /// <param name="jobtype">The type of this job.</param>
        /// <param name="owningDesignation"></param>
        protected Job(GameWorld owner, Actor actor, string jobtype, Designation owningDesignation)
            : base(actor,owner, true)
        {
            _jobType = jobtype;
            _isFinished = false;
            OwningDesignation = owningDesignation;
        }

        /// <summary>
        /// Creates a copy of this Job.
        /// </summary>
        /// <returns>A Job copy.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
