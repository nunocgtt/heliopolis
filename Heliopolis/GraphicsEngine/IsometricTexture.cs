using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heliopolis.Engine
{
    public class IsometricTexture
    {
        public string TextureName { get; set; }
        public Point Size { get; set; }
        public Point TexturePointOrigin { get; set; }
        public Point CenterPoint { get; set; }
        public int ZLevel { get; set; }
        public string TextureSheet { get; set; }
    }
}
