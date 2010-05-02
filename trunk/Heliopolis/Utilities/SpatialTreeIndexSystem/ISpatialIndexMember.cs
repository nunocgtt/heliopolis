using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities.SpatialTreeIndexSystem
{
    /// <summary>
    /// To be implemented by objects that need to be rendered.
    /// </summary>
    public interface ISpatialIndexMember
    {
        /// <summary>
        /// The position to render this object at.
        /// </summary>
        Point Position { get; set; }
    }
}
