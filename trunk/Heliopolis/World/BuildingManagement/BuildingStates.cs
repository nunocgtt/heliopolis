namespace Heliopolis.World.BuildingManagement
{
    /// <summary>
    /// The different states a building can be in.
    /// </summary>
    public enum BuildingStates
    {
        /// <summary>
        /// Currently getting constructed.
        /// </summary>
        UnderConstruction,
        /// <summary>
        /// Ready and constructed.
        /// </summary>
        Ready,
        /// <summary>
        /// In no state at the moment.
        /// </summary>
        None
    }
}