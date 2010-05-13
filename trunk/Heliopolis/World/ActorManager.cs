using System;
using System.Collections.Generic;
using System.Text;
using Heliopolis.Utilities.SpatialTreeIndexSystem;
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
        /// <param name="owner">The owning game world.</param>
        public ActorManager(GameWorld owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Spawns a new actor into the gameworld.
        /// </summary>
        /// <param name="actorType">The type of actor to spawn.</param>
        /// <param name="position">The starting position of the actor.</param>
        public void SpawnActor(string actorType, Point position)
        {
            Actor toAdd = ActorFactory.GetNewActor(actorType, position);
            Owner.SpatialTreeIndex.AddToSection(position, toAdd, new SpatialObjectKey() { ObjectType = SpatialObjectType.Actor, ObjectSubtype = actorType});
            LiveActors.Add(toAdd);
        }
    }
}
