using System;

namespace Heliopolis.World
{
    /// <summary>
    /// Represents a base game world object.
    /// </summary>
    /// <remarks>The GameWorldObject class is the base object that all game objects inherit from.</remarks>
    [Serializable]
    public class GameWorldObject : object 
    {
        private Guid _id;
        /// <summary>
        /// The game world that this object belongs to.
        /// </summary>
        protected GameWorld Owner;
        /// <summary>
        /// Initialises a new instance of the GameWorldObject class.
        /// </summary>
        /// <param name="owner">The owning GameWorld.</param>
        public GameWorldObject(GameWorld owner)
        {
            Owner = owner;
            _id = new Guid();
        }

        /// <summary>
        /// The unique identifier for this object.
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}
