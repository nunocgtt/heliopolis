using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;

namespace Heliopolis.World
{
    /// <summary>
    /// Represents a single job that an actor can perform.
    /// </summary>
    /// <remarks>Jobs are performed from within ActorStatePerformJob instances. Of importance is the
    /// JobParameter, which contains the various relevant information that the actor needs to actually
    /// get to and perform the job.</remarks>
    [Serializable]
    public class Job : GameWorldObject, System.ICloneable
    {
        private string jobType;
        private bool isFinished;

        private JobParameters jobParameters;

        /// <summary>
        /// If the current job is finished.
        /// </summary>
        public bool IsFinished
        {
            get { return isFinished; }
            set { isFinished = value; }
        }

        /// <summary>
        /// The type of this job.
        /// </summary>
        public string JobType
        {
            get { return jobType; }
            set { jobType = value; }
        }

        /// <summary>
        /// Any relevant parameters for this job.
        /// </summary>
        public JobParameters JobParameters
        {
            get { return jobParameters; }
            set { jobParameters = value; }
        }

        /// <summary>
        /// Initialises a new instance of the Job class.
        /// </summary>
        /// <param name="_owner">The owning game world.</param>
        /// <param name="_jobtype">The type of this job.</param>
        public Job(GameWorld _owner, string _jobtype) : base(_owner)
        {
            jobType = _jobtype;
            isFinished = false;
        }

        /// <summary>
        /// Peforms the job.
        /// </summary>
        public void Tick()
        {
            isFinished = true;
            switch (jobType)
            {
                case "mining":
                    //EnvironmentalJobParameters environmentalJobParameters = (EnvironmentalJobParameters)JobParameters;
                    //environmentalJobParameters.TargetTile.ResourceLeft -= 1;
                    //owner.ItemManager.SpawnItem(environmentalJobParameters.TargetTile.Resource, environmentalJobParameters.JobActor.Position);
                    //// we need to create some resource where the actor is
                    //isFinished = false;
                    //if (environmentalJobParameters.TargetTile.ResourceLeft == 0)
                    //{ 
                    //    EnvironmentTileFactory.SetToTemplate(environmentalJobParameters.TargetTile.ExhaustedTile, environmentalJobParameters.TargetTile);
                    //    isFinished = true;
                    //}
                    break;
                case "pickupitem":
                    MoveItemJobParameters moveItemJobParameters = (MoveItemJobParameters) jobParameters;
                    moveItemJobParameters.JobActor.PickupItem(moveItemJobParameters.TargetItem);
                    break;
                case "placeitem":
                    MoveItemJobParameters moveItemJobParametersPlaceItem = (MoveItemJobParameters) jobParameters;
                    moveItemJobParametersPlaceItem.JobActor.PlaceItem(moveItemJobParametersPlaceItem.TargetHolder);
                    break;
                case "construction":
                    BuildingJobParameters buildingJobParameters = (BuildingJobParameters) jobParameters;
                    buildingJobParameters.TargetBuilding.CompleteBuilding();
                    break;
            }
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

    /// <summary>
    /// Handles creation of the Job class, from a set of templates defined in an XML file.
    /// </summary>
    public class JobFactory
    {
        [NonSerialized]
        private static Dictionary<string, Job> jobTemplates = null;

        /// <summary>
        /// Loads Job templates from an XML file.
        /// </summary>
        /// <param name="xmlDoc">The xml file.</param>
        /// <param name="owner">The owning game world.</param>
        public static void LoadTemplatesFromXml(XmlDocument xmlDoc, GameWorld owner)
        {
            jobTemplates = new Dictionary<string, Job>();
            XmlNodeList itemNodes = xmlDoc.GetElementsByTagName("JobDefinition");
            foreach (XmlNode node in itemNodes)
            {
                XmlNode actionTime = node.SelectSingleNode("ActionTime");
                XmlNode producesItem = node.SelectSingleNode("ProducesItem");
                XmlNode targetTileResource = node.SelectSingleNode("TargetTileResource");
                Job addJob = new Job(owner, node.Attributes["name"].Value);
                AddTemplate(node.Attributes["name"].Value, addJob);
            }
        }

        /// <summary>
        /// Creates a new job from a template.
        /// </summary>
        /// <param name="templateName">The template to copy from.</param>
        /// <param name="_jobParameters">Any relevant parameters for this job.</param>
        /// <returns>A Job.</returns>
        public static Job GetNewJob(string templateName, JobParameters _jobParameters)
        {
            Job returnMe = (Job)jobTemplates[templateName].Clone();
            returnMe.JobParameters = _jobParameters;
            return returnMe;
        }

        private static void AddTemplate(string name, Job addJob)
        {
            jobTemplates.Add(name, addJob);
        }
    }
}