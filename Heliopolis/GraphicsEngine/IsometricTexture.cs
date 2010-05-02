using Microsoft.Xna.Framework;

namespace Heliopolis.GraphicsEngine
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
