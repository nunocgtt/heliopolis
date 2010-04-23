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
}
