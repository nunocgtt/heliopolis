using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

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

            // todo: load from XML file

            Textures.Add("grass", new IsometricTexture()
            {
                Size = new Point(32, 19),
                TextureName = "grass",
                ZLevel = 0,
                CenterPoint = new Point(16, 8),
                TexturePointOrigin = new Point(2 * 32, 0),
                TextureSheet = "floors"
            });
            Textures.Add("rock", new IsometricTexture()
            {
                Size = new Point(32, 19),
                TextureName = "rock",
                ZLevel = 0,
                CenterPoint = new Point(16, 8),
                TexturePointOrigin = new Point(4 * 32, 0),
                TextureSheet = "floors"
            });
            Textures.Add("tree1", new IsometricTexture()
            {
                Size = new Point(32, 32),
                TextureName = "tree1",
                ZLevel = 10,
                CenterPoint = new Point(16, 24),
                TexturePointOrigin = new Point(1 * 32, 0),
                TextureSheet = "trees"
            });
            Textures.Add("dwarf1", new IsometricTexture()
            {
                Size = new Point(32, 32),
                TextureName = "dwarf1",
                ZLevel = 10,
                CenterPoint = new Point(16, 25),
                TexturePointOrigin = new Point(1 * 32, 0),
                TextureSheet = "dwarves"
            });
            Textures.Add("selection1", new IsometricTexture()
            {
                Size = new Point(32, 19),
                TextureName = "selection1",
                ZLevel = 5,
                CenterPoint = new Point(16, 8),
                TexturePointOrigin = new Point(15 * 32, 1 * 20),
                TextureSheet = "floors"
            });

            Textures.Add("wood", new IsometricTexture()
            {
                Size = new Point(32, 32),
                TextureName = "wood",
                ZLevel = 6,
                CenterPoint = new Point(16, 24),
                TexturePointOrigin = new Point(0 * 32, 0 * 32),
                TextureSheet = "items"
            });
        }
    }
}