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
        private Dictionary<string, List<Designation>> designationsPendingPrereqs;

        /// <summary>
        /// Initialises a new instance of the DesignationManager class.
        /// </summary>
        /// <param name="_owner">The owning game world.</param>
        public DesignationManager(GameWorld _owner)
            : base(_owner)
        {
            designations = new Dictionary<string, List<Designation>>();
            designationsPendingPrereqs = new Dictionary<string, List<Designation>>();
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
            EnvironmentalJobParameters jobParameters = new EnvironmentalJobParameters(targetTile);
            Designation newDesignation = new Designation(owner, jobParameters, DesignationTypes.Simple);
            AddDesignation(jobType, newDesignation);
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
            if (!checkConstructionAble(targetPos, buildingToConstruct))
            {
                return false;
            }
            Building constructMe = owner.BuildingManager.StartBuildingConstruction(buildingToConstruct, targetPos);
            BuildingJobParameters buildingJobParameters = new BuildingJobParameters(constructMe);
            Designation newDesignation = new Designation(owner, buildingJobParameters, DesignationTypes.Construction);

            int j = 0;
            foreach (string item in BuildingFactory.BuildingTemplates[buildingToConstruct].RequiredMaterials)
            {
                for (int i = 0; i < BuildingFactory.BuildingTemplates[buildingToConstruct].RequiredMaterialAmount[j]; i++)
                {
                    MoveItemJobParameters moveItemJobParameters = new MoveItemJobParameters(item, constructMe);
                    Designation pickupItemDesignation = new Designation(owner, moveItemJobParameters, DesignationTypes.TransportItem);
                    newDesignation.AddPrerequisite(pickupItemDesignation);
                    AddDesignation("moveitem", pickupItemDesignation);
                }
                j++;
            }
            AddDesignation("construction", newDesignation);
            return true;
        }

        /// <summary>
        /// Add a new designation.
        /// </summary>
        /// <param name="jobType">The type of the job.</param>
        /// <param name="newDesignation">The designation to add.</param>
        public void AddDesignation(string jobType, Designation newDesignation)
        {
            if (!designations.ContainsKey(jobType))
            {
                designations.Add(jobType, new List<Designation>());
            }
            List<Designation> addInto = designations[jobType];
            addInto.Add(newDesignation);
        }

        /// <summary>
        /// Check for any available designations, that exist in a particular area.
        /// </summary>
        /// <param name="searcherAreaId">The area ID of the searcher.</param>
        /// <param name="jobType">The job type required.</param>
        /// <param name="searcherPosition">The position of the searcher.</param>
        /// <returns>Returns null if one doesnt exist, otherwise returns a Designation to perform.</returns>
        public Designation CheckAvailableDesignation(int searcherAreaId, string jobType, Point searcherPosition)
        {
            Designation returnMe = null;
            List<Designation> cleanUp = new List<Designation>();
            if (designations.ContainsKey(jobType))
            {
                if (designations[jobType].Count == 0)
                {
                    return null;
                }
                foreach (Designation d in designations[jobType])
                {
                    if (d.IsComplete)
                    {
                        cleanUp.Add(d);
                    }
                    else if (d.CanBeTaken(searcherAreaId, searcherPosition))
                    {
                        returnMe = d;
                        break;
                    }
                }
            }
            foreach (Designation d in cleanUp)
            {
                designations[jobType].Remove(d);
            }
            return returnMe;
        }
    }
}
