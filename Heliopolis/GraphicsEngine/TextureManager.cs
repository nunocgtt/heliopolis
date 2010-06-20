using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using ContentClasses;
using System.Linq;

namespace Heliopolis.GraphicsEngine
{
    public class TextureManager
    {
        public Dictionary<string, IsometricTexture> Textures = new Dictionary<string, IsometricTexture>();
        public Dictionary<string, Texture2D> TextureSheets = new Dictionary<string, Texture2D>();

        public TextureManager(ContentManager contentManager)
        {
            List<IsometricTexture> importList = contentManager.Load<List<IsometricTexture>>(@"GameWorldDefintion/textures");
            List<TextureSheet> textureSheet = contentManager.Load<List<TextureSheet>>(@"GameWorldDefintion/texturesheets");

            TextureSheets = textureSheet.ToDictionary(p => p.SheetName, q => contentManager.Load<Texture2D>(q.ContentFileName));
            Textures = importList.ToDictionary(p => p.TextureName);
        }
    }
}