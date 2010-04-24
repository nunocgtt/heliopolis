using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{
    public interface IRequiresAccess
    {
        void AccessChanged(bool canAccess, Point position);
    }
}
