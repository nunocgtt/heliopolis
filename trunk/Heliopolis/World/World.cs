using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Xml;
using System.IO;
using Heliopolis.Utilities;

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
        private static GameWorld instance;
        private Point worldSize = new Point(135, 135);
        private Point sectionSize = new Point(5,5);
        private Environment environment;
        private TimeSpan totalGameTime;
        private ActorManager actorManager;
        private DesignationManager designationManager;
        private ItemManager itemManager;
        private BuildingManager buildingManager;
        private SpatialTreeIndex spatialTreeIndex;
        private bool paused = true;

        static GameWorld()
        {
            instance = new GameWorld();
        }

        /// <summary>
        /// The GameWorld.
        /// </summary>
        public static GameWorld Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Sets the current game world instance to the one provided, and runs any initialisation routines.
        /// </summary>
        /// <param name="worldToLoad">The new game world to load.</param>
        public static void LoadNewGameWorld(GameWorld worldToLoad)
        {
            instance = worldToLoad;
            worldToLoad.LoadGameDescription();
            worldToLoad.Environment.InitialiseHelperClasses();
        }

        /// <summary>
        /// The spatial index used to access objects on the map.
        /// </summary>
        public SpatialTreeIndex SpatialTreeIndex
        {
            get { return spatialTreeIndex; }
        }

        /// <summary>
        /// Manages all the designations in the game.
        /// </summary>
        public DesignationManager DesignationManager
        {
            get { return designationManager; }
        }

        /// <summary>
        /// The size of the world environment.
        /// </summary>
        public Point WorldSize
        {
            get { return worldSize; }
            set { worldSize = value; }
        }

        /// <summary>
        /// The environment.
        /// </summary>
        public Environment Environment
        {
            get { return environment; }
        }

        /// <summary>
        /// Set to true to pause the game ticks.
        /// </summary>
        public bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }

        /// <summary>
        /// Manages all the items in the game.
        /// </summary>
        public ItemManager ItemManager
        {
            get { return itemManager; }
        }

        /// <summary>
        /// Manages all the buildings in the game.
        /// </summary>
        public BuildingManager BuildingManager
        {
            get { return buildingManager; }
        }

        /// <summary>
        /// The size for cutting up the game world into sections/areas.
        /// </summary>
        public Point SectionSize
        {
            get { return sectionSize; }
            set { sectionSize = value; }
        }

        /// <summary>
        /// Manages all the actors in the game.
        /// </summary>
        public ActorManager ActorManager
        {
            get { return actorManager; }
        }

        private GameWorld()
        {
            designationManager = new DesignationManager(this);
            buildingManager = new BuildingManager(this);
            itemManager = new ItemManager(this);
            actorManager = new ActorManager(this);
            environment = new Environment(worldSize, this);
            spatialTreeIndex = new SpatialTreeIndex(this.SectionSize, this.WorldSize, new int[] { 3, 3, 3, 3 });
            totalGameTime = TimeSpan.FromMilliseconds(0);
            LoadGameDescription();
        }

        /// <summary>
        /// Load the world.xml file for all the descriptions of game objects.
        /// </summary>
        public void LoadGameDescription()
        {
            environment.InitialiseEnvironment();
            XmlDocument doc = new XmlDocument();
            Stream s = this.GetType().Assembly.GetManifestResourceStream("ShadesGame.Gamedata.world.xml");
            using (s)
            {
                doc.Load(s);
                LoadFactoriesFromFile(doc);
            }
        }

        private void LoadFactoriesFromFile(XmlDocument xmlDoc)
        {
            ItemFactory.LoadTemplatesFromXml(xmlDoc, this);
            EnvironmentTileFactory.LoadTemplatesFromXml(xmlDoc, this);
            ActorFactory.LoadTemplatesFromXml(xmlDoc, this);
            JobFactory.LoadTemplatesFromXml(xmlDoc, this);
            BuildingFactory.LoadTemplatesFromXml(xmlDoc, this);
        }

        /// <summary>
        /// Loads up a test world.
        /// </summary>
        public void LoadTestWorld()
        {
            environment.LoadTestEnvironment();
            actorManager.SpawnActor("skeleton", new Point(0, 1));
        }

        /// <summary>
        /// Main game tick processing.
        /// </summary>
        /// <param name="timeSpan">The time since the last tick.</param>
        public void Tick(TimeSpan timeSpan)
        {
            if (paused)
                return;
            // Loop through all the actors and make them move
            totalGameTime = totalGameTime.Add(timeSpan);

            if (actorManager.ActorsByTime.Count > 0)
            {
                /* We want to process all the actors whos next absolute action time
                   is less than the current game time tick
                   NOTE: actors need to be processed in order of their action time
                   and in some cases an actor could be processed more than once if the game tick
                   is large enough. */

                // need the first value of the array
                //Actor first = firstActor();
                //bool keepLooping = true;
                List<TimeSpan> keysToReAdd = new List<TimeSpan>();

                // TODO: Refactor the timing logic into a TimedEventorManager class
                keysToReAdd.Clear();
                foreach (KeyValuePair<TimeSpan, Actor> kvp in actorManager.ActorsByTime)
                {
                    if (kvp.Key.CompareTo(totalGameTime) > 0)
                    {

                    }
                    else
                    {
                        kvp.Value.Tick(totalGameTime);
                        Actor reAddMe = kvp.Value;
                        keysToReAdd.Add(kvp.Key);
                    }
                }

                foreach (TimeSpan d in keysToReAdd)
                {
                    Actor reAddMe = actorManager.ActorsByTime[d];
                    actorManager.ActorsByTime.Remove(d);
                    // need to be careful about adding the same key
                    while (actorManager.ActorsByTime.ContainsKey(reAddMe.NextAbsoluteActionTime))
                    {
                        reAddMe.IncrementActionTime(TimeSpan.FromMilliseconds(1));
                    }
                    actorManager.ActorsByTime.Add(reAddMe.NextAbsoluteActionTime, reAddMe);
                    break;
                }
                // get first and reloop... maybe later. this will work fine as long as the actors
                // next move isnt inside this timeframe again
            }
        }
    }
}

