using Microsoft.Xna.Framework;

namespace Heliopolis.World.Environment
{
    /// <summary>
    /// Implement this interface if a class needs to be told when a particular <c>EnvironmentTile</c>s
    /// CanAccess property changes. Only is fired when an IRequiresAccessListener class has registered
    /// itself with an <c>EnvironmentTile</c>.
    /// </summary>
    public interface IRequiresAccessListener
    {
        /// <summary>
        /// Fired when the CanAccess property of an <c>EnvironmentTile</c> has changed.
        /// </summary>
        /// <param name="canAccess">if set to <c>true</c> then the tile is now accessible.</param>
        /// <param name="position">The position of the tile.</param>
        void AccessChanged(bool canAccess, Point position);
    }
}