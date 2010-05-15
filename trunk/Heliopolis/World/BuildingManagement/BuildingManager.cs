using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Heliopolis.World.BuildingManagement
{

    /// <summary>
    /// Manages buildings, such as creation and access to buildings.
    /// </summary>
    [Serializable]
    public class BuildingManager : GameWorldObject
    {
        private readonly List<Building> _buildings;

        /// <summary>
        /// All the buildings in the game world.
        /// </summary>
        public List<Building> Buildings
        {
            get { return _buildings; }
        }

        /// <summary>
        /// Initialises a new instance of the BuildingManager class.
        /// </summary>
        /// <param name="owner">The owning game world.</param>
        public BuildingManager(GameWorld owner) : base(owner)
        {
            _buildings = new List<Building>();
        }

        /// <summary>
        /// Creates a new building ready to start construction.
        /// </summary>
        /// <param name="buildingType">The type of building to create.</param>
        /// <param name="position">The position to create the building at.</param>
        /// <returns>A Building in the BuildingStates.UnderConstruction state.</returns>
        public Building StartBuildingConstruction(string buildingType, Point position)
        {
            Building spawnMe = BuildingFactory.GetNewBuilding(buildingType, position);
            _buildings.Add(spawnMe);
            spawnMe.StartBuilding();
            return spawnMe;
        }

        public Building FindStorage()
        {
            return null;
        }

        //public Building SpawnBuilding(string buildingType, Point position)
        //{
        //    Building spawnMe = BuildingFactory.GetNewBuilding(buildingType, position);
        //    spawnMe.BuildingState = BuildingStates.Ready;
        //    buildings.Add(spawnMe);
        //    return spawnMe;
        //}
    }
}
