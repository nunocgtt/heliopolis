using System;
using System.Collections.Generic;
using System.Linq;
using ContentClasses;
using Heliopolis.World.Environment;
using Heliopolis.World.BuildingManagement;
using Microsoft.Xna.Framework;

namespace Heliopolis.World.JobSystem
{

    /// <summary>
    /// Manages all the designations in the gameworld.
    /// </summary>
    [Serializable]
    public class DesignationManager : GameWorldObject
    {
        private readonly Dictionary<string, List<Designation>> _designations;
        private readonly Dictionary<string, List<Designation>> _designationsPending;

        /// <summary>
        /// Initialises a new instance of the DesignationManager class.
        /// </summary>
        /// <param name="owner">The owning game world.</param>
        public DesignationManager(GameWorld owner)
            : base(owner)
        {
            _designations = new Dictionary<string, List<Designation>>();
            _designationsPending = new Dictionary<string, List<Designation>>();
        }

        private void AddKey(string newKey)
        {
            if (!_designations.ContainsKey(newKey))
            {
                _designations.Add(newKey, new List<Designation>());
            }
            if (!_designationsPending.ContainsKey(newKey))
            {
                _designationsPending.Add(newKey, new List<Designation>());
            }
        }

        /// <summary>
        /// Add a new designation.
        /// </summary>
        /// <param name="newDesignation">The designation to add.</param>
        public void AddDesignation(Designation newDesignation)
        {
            AddKey(newDesignation.JobType);
            // The designation will add itself into the correct dictionary when created
        }

        // These three methods are used by the designations to update their availability status

        private void CheckJobTypeInDictionary(string jobType)
        {
            if (!_designationsPending.ContainsKey(jobType))
            {
                _designationsPending[jobType] = new List<Designation>();
                _designations[jobType] = new List<Designation>();
            }
        }

        public void MakeDesignationAvailable(Designation availableDesignation)
        {
            CheckJobTypeInDictionary(availableDesignation.JobType);
            if (_designationsPending[availableDesignation.JobType].Contains(availableDesignation))
                _designationsPending[availableDesignation.JobType].Remove(availableDesignation);
            if (!_designations[availableDesignation.JobType].Contains(availableDesignation))
                _designations[availableDesignation.JobType].Add(availableDesignation);
        }

        public void MakeDesignationUnavailable(Designation unavailableDesignation)
        {
            CheckJobTypeInDictionary(unavailableDesignation.JobType);
            if (!_designationsPending[unavailableDesignation.JobType].Contains(unavailableDesignation))
                _designationsPending[unavailableDesignation.JobType].Add(unavailableDesignation);
            if (_designations[unavailableDesignation.JobType].Contains(unavailableDesignation))
                _designations[unavailableDesignation.JobType].Remove(unavailableDesignation);
        }

        public void DesignationCompleted(Designation completedDesignation)
        {
            if (_designations[completedDesignation.JobType].Contains(completedDesignation))
                _designations[completedDesignation.JobType].Remove(completedDesignation);
            if (_designationsPending[completedDesignation.JobType].Contains(completedDesignation))
                _designationsPending[completedDesignation.JobType].Remove(completedDesignation);
        }

        /// <summary>
        /// Check to see if construction is possible.
        /// </summary>
        /// <param name="targetPos">Where to construct the building.</param>
        /// <param name="buildingToConstruct">The building to construct.</param>
        /// <returns>Returns true if the building can be constructed.</returns>
        public bool CheckConstructionAble(Point targetPos, string buildingToConstruct)
        {
            BuildingTemplate toBeConstructed = BuildingFactory.BuildingTemplates[buildingToConstruct];
            for (int i = 0; i < toBeConstructed.Size.X; i++)
            {
                for (int j = 0; j < toBeConstructed.Size.Y; j++)
                {
                    if (!Owner.Environment[targetPos.X + i, targetPos.Y + j].CanAccess)
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
        /// <param name="designationToTake">The designation to take.</param>
        /// <returns>Returns null if one doesnt exist, otherwise returns a Designation to perform.</returns>
        public bool CheckAvailableDesignation(int searcherAreaId, string jobType, Point searcherPosition, out Designation designationToTake)
        {
            if (_designations.ContainsKey(jobType))
            {
                foreach (Designation d in _designations[jobType].Where(d => d.CanBeTakenFromArea(searcherAreaId)))
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
