using Microsoft.Xna.Framework;
using System;

namespace ContentClasses
{
    [Serializable]
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
