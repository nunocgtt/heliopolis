﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Heliopolis.Engine
{
    public interface IIsometricTileProvider
    {
        List<string> GetTexturesToDraw(Point position);
    }
}
