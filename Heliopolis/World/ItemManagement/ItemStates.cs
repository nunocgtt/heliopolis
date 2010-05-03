namespace Heliopolis.World.ItemManagement
{
    /// <summary>
    /// The varios states an Item can exist in.
    /// </summary>
    public enum ItemStates
    {
        /// <summary>
        /// The item is on the ground.
        /// </summary>
        OnGround,
        /// <summary>
        /// The item is being carried by an actor.
        /// </summary>
        BeingCarried,
        /// <summary>
        /// The item is in a building.
        /// </summary>
        InStorage,
        /// <summary>
        /// The item in an actor's storage/backpack.
        /// </summary>
        InBackpack
    }
}