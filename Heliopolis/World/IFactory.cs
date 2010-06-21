using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Heliopolis.World
{
    public interface IFactory
    {
        void LoadTemplatesFromContent(ContentManager contentManager, GameWorld owner, string contentFile);
    }
}
