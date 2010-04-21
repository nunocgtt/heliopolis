using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities
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
