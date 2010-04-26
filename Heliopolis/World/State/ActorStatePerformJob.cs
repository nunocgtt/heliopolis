using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heliopolis.World.State
{
    /// <summary>
    /// Represents an actor state for peforming a Job.
    /// </summary>
    /// <remarks>The actual peformance of the job is handled in the Job class Job.Tick()</remarks>
    [Serializable]
    public class ActorStatePerformJob : ActorState
    {
        private Job myJob;
        /// <summary>
        /// Initialises a new instance of the ActorStatePerformJob class.
        /// </summary>
        /// <param name="_myActor">The actor who this state belongs to.</param>
        /// <param name="_myJob">The job to perform.</param>
        /// <param name="_owner">The owning game world.</param>
        public ActorStatePerformJob(Actor _myActor, Job _myJob, GameWorld _owner)
            : base(_myActor, _owner)
        {
            actionType = _myJob.JobType;
            myJob = _myJob;
        }

        /// <summary>
        /// Peforms the state action.
        /// </summary>
        public override void Tick()
        {
            myJob.Tick();
            base.Tick();
        }

        protected override bool checkFinishedState()
        {
            return (myJob.IsFinished);
        }
    }

}
