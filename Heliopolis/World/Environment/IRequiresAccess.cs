using Microsoft.Xna.Framework;

namespace Heliopolis.World.Environment
{
    public interface IRequiresAccess
    {
        void AccessChanged(bool canAccess, Point position);
    }
}
