using System;
using System.Collections.Generic;
using Heliopolis.Utilities.SpatialTreeIndexSystem;
using Heliopolis.World.BuildingManagement;
using Heliopolis.World.Environment;
using Heliopolis.World.ItemManagement;
using Heliopolis.World.JobSystem;
using Microsoft.Xna.Framework;
using System.Xml;
using System.IO;
using Heliopolis.Utilities;
using Microsoft.Xna.Framework.Content;

namespace Heliopolis.World
{
    /// <summary>
    /// Represents a whole Game World.
    /// </summary>
    /// <remarks>This is the top level object for a game, and contains a number of managers that
    /// manage each class. It also contains the game environment. This object is a singleton
    /// and hence you can not call its constructor. It must be accessed through the GameWorld.Instance
    /// static property.</remarks>
    [Serializable]
    public class GameWorld
    {
        private static GameWorld _instance;
        private Point _worldSize = new Point(135, 135);
        private Point _sectionSize = new Point(5,5);
        private readonly Environment.Environment _environment;
        private readonly ActorManager _actorManager;
        private readonly DesignationManager _designationManager;
        private readonly ItemManager _itemManager;
        private readonly BuildingManager _buildingManager;
        private readonly SpatialTreeIndex _spatialTreeIndex;
        private readonly FactoryManager _factoryManager;
        public TimedEventManager TimedEventManager { get; set; }

        static GameWorld()
        {
            _instance = new GameWorld();
        }

        /// <summary>
        /// The GameWorld.
        /// </summary>
        public static GameWorld Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Sets the current game world instance to the one provided, and runs any initialisation routines.
        /// </summary>
        /// <param name="worldToLoad">The new game world to load.</param>
        public static void LoadNewGameWorld(GameWorld worldToLoad)
        {
            throw new NotSupportedException("Yet to be properly implemented.");
            _instance = worldToLoad;
            //worldToLoad.LoadGameDescription();
            worldToLoad.Environment.InitialiseHelperClasses();
        }

        /// <summary>
        /// The spatial index used to access objects on the map.
        /// </summary>
        public SpatialTreeIndex SpatialTreeIndex
        {
            get { return _spatialTreeIndex; }
        }

        /// <summary>
        /// Manages all the designations in the game.
        /// </summary>
        public DesignationManager DesignationManager
        {
            get { return _designationManager; }
        }

        /// <summary>
        /// The size of the world environment.
        /// </summary>
        public Point WorldSize
        {
            get { return _worldSize; }
            set { _worldSize = value; }
        }

        /// <summary>
        /// The environment.
        /// </summary>
        public Environment.Environment Environment
        {
            get { return _environment; }
        }

        /// <summary>
        /// Manages all the items in the game.
        /// </summary>
        public ItemManager ItemManager
        {
            get { return _itemManager; }
        }

        /// <summary>
        /// Manages all the buildings in the game.
        /// </summary>
        public BuildingManager BuildingManager
        {
            get { return _buildingManager; }
        }

        /// <summary>
        /// The size for cutting up the game world into sections/areas.
        /// </summary>
        public Point SectionSize
        {
            get { return _sectionSize; }
            set { _sectionSize = value; }
        }

        /// <summary>
        /// Manages all the actors in the game.
        /// </summary>
        public ActorManager ActorManager
        {
            get { return _actorManager; }
        }

        private GameWorld()
        {
            _designationManager = new DesignationManager(this);
            _buildingManager = new BuildingManager(this);
            _itemManager = new ItemManager(this);
            _actorManager = new ActorManager(this);
            _environment = new Environment.Environment(_worldSize, this);
            _spatialTreeIndex = new SpatialTreeIndex(WorldSize, new [] { 3, 3, 3, 3 }, new [] { 3, 3, 3, 3 });
            TimedEventManager = new TimedEventManager(this);
            _factoryManager = new FactoryManager(this);
        }

        /// <summary>
        /// Load the world.xml file for all the descriptions of game objects.
        /// </summary>
        public void LoadGameDescription(ContentManager contentManager)
        {
            _environment.InitialiseEnvironment();
            _factoryManager.InitializeFactories(contentManager);
        }

        /// <summary>
        /// Loads up a test world.
        /// </summary>
        public void LoadTestWorld()
        {
            _environment.LoadTestEnvironment();
            _actorManager.SpawnActor("dood", new Point(0, 0));
            _actorManager.SpawnActor("dood", new Point(0, 0));
            _actorManager.SpawnActor("dood", new Point(0, 0));
            _actorManager.SpawnActor("dood", new Point(0, 0));
            _actorManager.SpawnActor("dood", new Point(0, 0));
        }

        /// <summary>
        /// Main game tick processing.
        /// </summary>
        /// <param name="timeSpan">The time since the last tick.</param>
        public void Tick(GameTime timeSpan)
        {
            TimedEventManager.Tick(timeSpan);
        }
    }
}

