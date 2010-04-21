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
        public List<Actor> LiveActors = new List<Actor>();

        /// <summary>
        /// Initialises a new instance of the ActorManager class.
        /// </summary>
        /// <param name="_owner">The owning game world.</param>
        public ActorManager(GameWorld _owner)
            : base(_owner)
        {
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
            owner.TimedEventManager.StartTimedAtCurrentTime(toAdd);
            LiveActors.Add(toAdd);
        }
    }
}
