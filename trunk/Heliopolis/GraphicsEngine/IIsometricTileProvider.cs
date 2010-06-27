using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Heliopolis.GraphicsEngine
{
    public interface IIsometricTileProvider
    {
        List<TextureToDraw> GetTexturesToDraw(Point position);
    }
}
