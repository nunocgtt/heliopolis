using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Heliopolis.World
{
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
