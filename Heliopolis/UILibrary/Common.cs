using Microsoft.Xna.Framework.Graphics;
using System;
using System.Globalization;

namespace Heliopolis.UILibrary
{
    public static class Common
    {
        public static Color StringToColor(string colorValue)
        {
            return new Color(Byte.Parse(colorValue.Substring(0,2), NumberStyles.HexNumber),
                Byte.Parse(colorValue.Substring(2, 2), NumberStyles.HexNumber),
                Byte.Parse(colorValue.Substring(4, 2), NumberStyles.HexNumber));
        }
    }
    public struct UIEvent
    {
        public Panel SourcePanel;
        public string EventMessage;
    }

    public delegate void UIEventDelegate(UIEvent sourceEvent);

    public struct UIEventHandler
    {
        public string EventMessage;
        public UIEventDelegate EventDelegate;
    }

    public enum UIAnchor
    {
        Top,
        TopLeft,
        TopRight,
        Center,
        CenterLeft,
        CenterRight,
        Bottom,
        BottomLeft,
        BottomRight
    }

    

    public class UIBackdropTexture
    {
        public Texture2D Texture;
        public int TileWidth;
        public int TileHeight;

        public UIBackdropTexture(Texture2D texture, int tileWidth, int tileHeight)
        {
            Texture = texture;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }
    }

}