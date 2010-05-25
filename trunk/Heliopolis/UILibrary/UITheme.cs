using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace Heliopolis.UILibrary
{
    public class UITheme
    {
        public XmlDocument ThemeXML { get; set; }
        public UserInterface UserInterface { get; set; }

        public Dictionary<string, UIBackdropTexture> BackdropTextures;
        public Dictionary<string, Texture2D> Textures;
        public Dictionary<string, Color> Colors;
        public Dictionary<string, SpriteFont> Fonts;

        public UITheme(XmlDocument themeXML, UserInterface userInterface)
        {
            ThemeXML = themeXML;
            UserInterface = userInterface;

            BackdropTextures = new Dictionary<string, UIBackdropTexture>();
            Textures = new Dictionary<string, Texture2D>();
            Colors = new Dictionary<string, Color>();
            Fonts = new Dictionary<string, SpriteFont>();

            LoadFromXML();
        }

        protected virtual void LoadFromXML()
        {
            XmlNodeList backdropTextureNodes = ThemeXML.SelectNodes(@"/Theme/BackdropTextures/BackdropTexture");

            foreach (XmlNode backdropTextureNode in backdropTextureNodes)
            {
                LoadBackdropTexture(backdropTextureNode);
            }

            XmlNodeList textureNodes = ThemeXML.SelectNodes(@"/Theme/Textures/Texture");

            foreach (XmlNode textureNode in textureNodes)
            {
                LoadTexture(textureNode);
            }

            XmlNodeList colorNodes = ThemeXML.SelectNodes(@"/Theme/Colors/Color");

            foreach (XmlNode colorNode in colorNodes)
            {
                LoadColor(colorNode);
            }

            XmlNodeList fontNodes = ThemeXML.SelectNodes(@"/Theme/Fonts/Font");

            foreach (XmlNode fontNode in fontNodes)
            {
                LoadFont(fontNode);
            }
        }

        protected virtual void LoadBackdropTexture(XmlNode backdropTextureNode)
        {
            if (backdropTextureNode != null)
            {
                XmlNode nameAttribute = backdropTextureNode.SelectSingleNode(@"@name");
                XmlNode textureAttribute = backdropTextureNode.SelectSingleNode(@"@texture");
                XmlNode tileWidthAttribute = backdropTextureNode.SelectSingleNode(@"@tileWidth");
                XmlNode tileHeightAttribute = backdropTextureNode.SelectSingleNode(@"@tileHeight");

                string name = (nameAttribute != null) ? nameAttribute.Value : "";
                string texture = (textureAttribute != null) ? textureAttribute.Value : "";

                if (name.Length > 0 && texture.Length > 0)
                {
                    if (!BackdropTextures.Keys.Contains(name))
                    {
                        int tileWidth = 0;
                        int tileHeight = 0;

                        if (tileWidthAttribute != null)
                        {
                            if (!Int32.TryParse(tileWidthAttribute.Value, out tileWidth))
                            {
                                tileWidth = 0;
                            }
                        }

                        if (tileHeightAttribute != null)
                        {
                            if (!Int32.TryParse(tileHeightAttribute.Value, out tileHeight))
                            {
                                tileHeight = 0;
                            }
                        }

                        if (name.Length > 0 && texture.Length > 0 && tileWidth > 0 && tileHeight > 0)
                        {
                            try
                            {
                                Texture2D backgroundTexture = UserInterface.Game.Content.Load<Texture2D>(texture);
                                UIBackdropTexture backdropTexture = new UIBackdropTexture(backgroundTexture, tileWidth, tileHeight);

                                if (!BackdropTextures.Keys.Contains(name))
                                {
                                    BackdropTextures.Add(name, backdropTexture);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Theme load failed during backdrop texture load for the following texture: " + texture, ex);
                            }
                        }
                    }
                }
            }
        }

        protected virtual void LoadTexture(XmlNode textureNode)
        {
            if (textureNode != null)
            {
                XmlNode nameAttribute = textureNode.SelectSingleNode(@"@name");
                XmlNode textureAttribute = textureNode.SelectSingleNode(@"@texture");
                XmlNode activeAttribute = textureNode.SelectSingleNode(@"@active");

                string name = (nameAttribute != null) ? nameAttribute.Value : "";
                string asset = (textureAttribute != null) ? textureAttribute.Value : "";

                bool active = false;

                if (activeAttribute != null)
                {
                    active = (activeAttribute.Value.ToLower() == "true") ? true : false;
                }

                if (name.Length > 0 && asset.Length > 0)
                {
                    try
                    {
                        Texture2D texture = UserInterface.Game.Content.Load<Texture2D>(asset);

                        if (!Textures.Keys.Contains(name))
                        {
                            Textures.Add(name, texture);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Texture load failed for the following texture: " + asset, ex);
                    }
                }
            }
        }

        protected virtual void LoadColor(XmlNode colorNode)
        {
            if (colorNode != null)
            {
                XmlNode nameAttribute = colorNode.SelectSingleNode(@"@name");
                XmlNode htmlColorNode = colorNode.FirstChild;

                string name = (nameAttribute != null) ? nameAttribute.Value : "";

                if (name.Length > 0 && !Colors.Keys.Contains(name))
                {
                    if (htmlColorNode != null)
                    {
                        Color color = Color.Black;

                        string htmlColorString = "";

                        try
                        {
                            htmlColorString = colorNode.FirstChild.Value;
                            Color htmlColor = Common.StringToColor(htmlColorString);
                            color = new Color(htmlColor.R, htmlColor.G, htmlColor.B);

                            if (!Colors.Keys.Contains(name))
                            {
                                Colors.Add(name, color);
                            }
                        }
                        catch(Exception ex)
                        {
                            throw new Exception("Theme load failed during color load for the following color: " + htmlColorString, ex);
                        }
                    }
                }
            }
        }

        protected virtual void LoadFont(XmlNode fontNode)
        {
            if (fontNode != null)
            {
                XmlNode nameAttribute = fontNode.SelectSingleNode(@"@name");
                XmlNode fontPathNode = fontNode.FirstChild;

                string name = (nameAttribute != null) ? nameAttribute.Value : "";

                if (name.Length > 0 && !Fonts.Keys.Contains(name))
                {
                    if (fontPathNode != null)
                    {
                        string fontPath = "";

                        try
                        {
                            fontPath = fontPathNode.Value;

                            SpriteFont font = UserInterface.Game.Content.Load<SpriteFont>(fontPath);

                            if (!Fonts.Keys.Contains(name))
                            {
                                Fonts.Add(name, font);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Theme load failed during font load for the following font: " + fontPath, ex);
                        }
                    }
                }
            }
        }

        public UIBackdropTexture GetBackdropTexture(string name)
        {
            UIBackdropTexture backdropTexture = null;

            if (BackdropTextures.Keys.Contains(name))
            {
                backdropTexture = BackdropTextures[name];
            }

            return backdropTexture;
        }

        public Texture2D GetTexture(string name)
        {
            Texture2D texture = null;

            if (Textures.Keys.Contains(name))
            {
                texture = Textures[name];
            }

            return texture;
        }

        public Color GetColor(string name)
        {
            Color color = Color.Black;

            if (Colors.Keys.Contains(name))
            {
                color = Colors[name];
            }

            return color;
        }

        public SpriteFont GetFont(string name)
        {
            SpriteFont font = null;

            if (Fonts.Keys.Contains(name))
            {
                font = Fonts[name];
            }

            return font;
        }

    }
}
