using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Heliopolis.Utilities;

namespace Heliopolis.World
{
    /// <summary>
    /// Manages Actor creation and access to actors that exist in the game world.
    /// </summary>
    [Serializable]
    public class ActorManager : GameWorldObject
    {
        private SortedDictionary<TimeSpan, Actor> actorsByTime = new SortedDictionary<TimeSpan, Actor>();

        /// <summary>
        /// Initialises a new instance of the ActorManager class.
        /// </summary>
        /// <param name="_owner">The owning game world.</param>
        public ActorManager(GameWorld _owner)
            : base(_owner)
        {

        }

        /// <summary>
        /// Public access to the internal actors by time. This will get moved out to a TimedEventor manager.
        /// </summary>
        public SortedDictionary<TimeSpan, Actor> ActorsByTime
        {
            get { return actorsByTime; }
            set { actorsByTime = value; }
        }

        /// <summary>
        /// Returns the first actor in the list that is sorted by next action time.
        /// </summary>
        /// <returns></returns>
        public Actor FirstActor()
        {
            Actor firstActor = null;
            SortedDictionary<TimeSpan, Actor>.ValueCollection valueCol = actorsByTime.Values;
            foreach (Actor a in valueCol)
            {
                firstActor = a;
                break;
            }
            return firstActor;
        }

        /// <summary>
        /// Spawns a new actor into the gameworld.
        /// </summary>
        /// <param name="ActorType">The type of actor to spawn.</param>
        /// <param name="position">The starting position of the actor.</param>
        public void SpawnActor(string ActorType, Point position)
        {
            Actor toAdd = ActorFactory.GetNewActor(ActorType, position);
            owner.SpatialTreeIndex.AddToSection(position, toAdd, SpatialObjectType.Actor, "");
            actorsByTime.Add(TimeSpan.FromMilliseconds(1), toAdd);
        }

    }
}
