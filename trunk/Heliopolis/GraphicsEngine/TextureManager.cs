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
        public Dictionary<string, string> TextureSheetNames = new Dictionary<string, string>();

        public TextureManager(ContentManager contentManager)
        {
            TextureSheetNames.Add("floors", "Sprites/floors");
            TextureSheetNames.Add("trees", "Sprites/trees");
            TextureSheetNames.Add("dwarves", "Sprites/dwarves");
            TextureSheetNames.Add("items", "Sprites/items");

            foreach (KeyValuePair<string, string> kvp in TextureSheetNames)
                TextureSheets.Add(kvp.Key, contentManager.Load<Texture2D>(kvp.Value));

            List<IsometricTexture> importList = contentManager.Load<List<IsometricTexture>>(@"GameWorldDefintion/textures");

            Textures = importList.ToDictionary(p => p.TextureName);
        }
    }
}