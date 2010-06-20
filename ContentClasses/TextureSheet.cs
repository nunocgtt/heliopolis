using System;
using Microsoft.Xna.Framework;

namespace ContentClasses
{
    [Serializable]
    public class TextureSheet
    {
        public string SheetName { get; set; }
        public string ContentFileName { get; set; }
    }
}