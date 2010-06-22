using System;
using Microsoft.Xna.Framework;

namespace ContentClasses
{
    [Serializable]
    public class MouseCursor
    {
        public string Name { get; set; }
        public string MouseTexture { get; set; }
        public Point CenterPoint { get; set; }
    }
}