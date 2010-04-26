using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{

    /// <summary>
    /// Manages all the designations in the gameworld.
    /// </summary>
    [Serializable]
    public class DesignationManager : GameWorldObject
    {
        private Dictionary<string, List<Designation>> designations;
        private Dictionary<string, List<Designation>> designationsPending;

        /// <summary>
        /// Initialises a new instance of the DesignationManager class.
        /// </summary>
        /// <param name="_owner">The owning game world.</param>
        public DesignationManager(GameWorld _owner)
            : base(_owner)
        {
            designations = new Dictionary<string, List<Designation>>();
            designationsPending = new Dictionary<string, List<Designation>>();
        }

        private void addKey(string newKey)
        {
            if (!designations.ContainsKey(newKey))
            {
                designations.Add(newKey, new List<Designation>());
            }
            if (!designationsPending.ContainsKey(newKey))
            {
                designationsPending.Add(newKey, new List<Designation>());
            }
        }

        /// <summary>
        /// Add a new designation.
        /// </summary>
        /// <param name="jobType">The type of the job.</param>
        /// <param name="newDesignation">The designation to add.</param>
        public void AddDesignation(Designation newDesignation)
        {
            addKey(newDesignation.JobType);
            // The designation will add itself into the correct dictionary when created
        }

        // These three methods are used by the designations to update their availability status

        public void MakeDesignationAvailable(Designation availableDesignation)
        {
            if (designationsPending[availableDesignation.JobType].Contains(availableDesignation))
                designationsPending[availableDesignation.JobType].Remove(availableDesignation);
            if (!designations[availableDesignation.JobType].Contains(availableDesignation))
                designations[availableDesignation.JobType].Add(availableDesignation);
        }

        public void MakeDesignationUnavailable(Designation unavailableDesignation)
        {
            if (!designationsPending[unavailableDesignation.JobType].Contains(unavailableDesignation))
                designationsPending[unavailableDesignation.JobType].Add(unavailableDesignation);
            if (designations[unavailableDesignation.JobType].Contains(unavailableDesignation))
                designations[unavailableDesignation.JobType].Remove(unavailableDesignation);
        }

        public void DesignationCompleted(Designation completedDesignation)
        {
            if (designations[completedDesignation.JobType].Contains(completedDesignation))
                designations[completedDesignation.JobType].Remove(completedDesignation);
            if (designationsPending[completedDesignation.JobType].Contains(completedDesignation))
                designationsPending[completedDesignation.JobType].Remove(completedDesignation);
        }

        // Move these elsewhere, maybe into static designation methods
        /// <summary>
        /// Add a new harvesting designation.
        /// </summary>
        /// <param name="targetPos">The position of the tile to harvest.</param>
        /// <param name="jobType">The type of harvesting.</param>
        /// <returns>Returns true if the designation was created.</returns>
        public bool AddMiningDesignation(Point targetPos, string jobType)
        {
            EnvironmentTile targetTile = owner.Environment[targetPos];
            if (!targetTile.CanAccess)
            {
                AddSimpleDesignation(targetTile, jobType);
                return true;
            }
            else
                return false;
        }

        // Move these elsewhere, maybe into static designation methods
        /// <summary>
        /// Adds in a simple designation.
        /// </summary>
        /// <param name="targetTile">The position of the job to perform at.</param>
        /// <param name="jobType">The job to perform.</param>
        public void AddSimpleDesignation(EnvironmentTile targetTile, string jobType)
        {
            //EnvironmentalJobParameters jobParameters = new EnvironmentalJobParameters(targetTile);
            //Designation newDesignation = new Designation(owner, jobParameters, DesignationTypes.Simple);
            //AddDesignation(jobType, newDesignation);
        }

        /// <summary>
        /// Check to see if construction is possible.
        /// </summary>
        /// <param name="targetPos">Where to construct the building.</param>
        /// <param name="buildingToConstruct">The building to construct.</param>
        /// <returns>Returns true if the building can be constructed.</returns>
        public bool checkConstructionAble(Point targetPos, string buildingToConstruct)
        {
            Building toBeConstructed = BuildingFactory.BuildingTemplates[buildingToConstruct];
            for (int i = 0; i < toBeConstructed.Size.X; i++)
            {
                for (int j = 0; j < toBeConstructed.Size.Y; j++)
                {
                    if (!owner.Environment[targetPos.X + i, targetPos.Y + j].CanAccess)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a new construction designation.
        /// </summary>
        /// <param name="targetPos">The position of the building.</param>
        /// <param name="buildingToConstruct">The type of building requiring construction.</param>
        /// <returns>Returns true if the designation was created.</returns>
        public bool AddConstructionDesignation(Point targetPos, string buildingToConstruct)
        {
            //if (!checkConstructionAble(targetPos, buildingToConstruct))
            //{
            //    return false;
            //}
            //Building constructMe = owner.BuildingManager.StartBuildingConstruction(buildingToConstruct, targetPos);
            //BuildingJobParameters buildingJobParameters = new BuildingJobParameters(constructMe);
            //Designation newDesignation = new Designation(owner, buildingJobParameters, DesignationTypes.Construction);

            //int j = 0;
            //foreach (string item in BuildingFactory.BuildingTemplates[buildingToConstruct].RequiredMaterials)
            //{
            //    for (int i = 0; i < BuildingFactory.BuildingTemplates[buildingToConstruct].RequiredMaterialAmount[j]; i++)
            //    {
            //        MoveItemJobParameters moveItemJobParameters = new MoveItemJobParameters(item, constructMe);
            //        Designation pickupItemDesignation = new Designation(owner, moveItemJobParameters, DesignationTypes.TransportItem);
            //        newDesignation.AddPrerequisite(pickupItemDesignation);
            //        AddDesignation("moveitem", pickupItemDesignation);
            //    }
            //    j++;
            //}
            //AddDesignation("construction", newDesignation);
            return true;
        }


        /// <summary>
        /// Check for any available designations, that exist in a particular area.
        /// </summary>
        /// <param name="searcherAreaId">The area ID of the searcher.</param>
        /// <param name="jobType">The job type required.</param>
        /// <param name="searcherPosition">The position of the searcher.</param>
        /// <returns>Returns null if one doesnt exist, otherwise returns a Designation to perform.</returns>
        public bool CheckAvailableDesignation(int searcherAreaId, string jobType, Point searcherPosition, out Designation designationToTake)
        {
            foreach (Designation d in designations[jobType])
            {
                if (d.CanBeTakenFromArea(searcherAreaId))
                {
                    designationToTake = d;
                    return true;
                }
            }
            designationToTake = null;
            return false;
        }
    }
}
