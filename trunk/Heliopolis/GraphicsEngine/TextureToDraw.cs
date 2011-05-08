using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heliopolis.GraphicsEngine
{
    public class TextureToDraw
    {
        public TextureToDraw(string textureName)
        {
            TextureName = textureName;
            DrawColor = Color.White;
        }

        public TextureToDraw(string textureName, Color drawColor)
        {
            TextureName = textureName;
            DrawColor = drawColor;
        }

        public string TextureName { get; set; }
        public Color DrawColor { get; set; }
    }
}
