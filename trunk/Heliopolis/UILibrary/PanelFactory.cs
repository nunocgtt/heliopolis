using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heliopolis.UILibrary
{
    public class PanelFactory
    {
        private Panel _panelCreate;
        private XmlNode _xmlDocument;

        public Panel CreateNewPanelByTypeWithParent(XmlNode xmlDocument, UserInterface userInterface, Type objectType, Panel parentPanel)
        {
            _panelCreate = (Panel)Activator.CreateInstance(objectType);
            _xmlDocument = xmlDocument;
            Initialize(parentPanel, userInterface);
            return _panelCreate;
        }

        public Panel CreateNewPanelByType(XmlNode xmlDocument, UserInterface userInterface, Type objectType)
        {
            _panelCreate = (Panel) Activator.CreateInstance(objectType);
            _xmlDocument = xmlDocument;
            Initialize(null, userInterface);
            return _panelCreate;
        }

        protected void Initialize(Panel parent, UserInterface userInterface)
        {
            _panelCreate.Parent = parent;
            _panelCreate.UserInterface = userInterface;
            _panelCreate.Panels = new Dictionary<string, Panel>();
            _panelCreate.HasFocus = false;

            bool isIPanel = _panelCreate is IPanel;

            LoadFromXML();
            
            LoadEventHandlers();
            if (isIPanel)
                ((IPanel)_panelCreate).LoadEventHandlers(_xmlDocument);
            LoadDesignElements();
            if (isIPanel)
                ((IPanel)_panelCreate).LoadDesignElements();
            LoadDrawRectangle();
            if (isIPanel)
                ((IPanel)_panelCreate).LoadDrawRectangle(_xmlDocument);
            LoadPanels();

            _panelCreate.EnableClipping = false;

            if (isIPanel)
                ((IPanel)_panelCreate).Load(_xmlDocument);
        }

        protected virtual void LoadFromXML()
        {
            XmlNode idAttribute = _xmlDocument.SelectSingleNode(@"@id");
            _panelCreate.PanelLayoutType = LayoutType.Absolute;

            if (idAttribute != null)
            {
                _panelCreate.ID = idAttribute.Value;

                XmlNode enabledAttribute = _xmlDocument.SelectSingleNode(@"@enabled");
                XmlNode visibleAttribute = _xmlDocument.SelectSingleNode(@"@visible");
                XmlNode layoutType = _xmlDocument.SelectSingleNode(@"@layout");
                XmlNode groupId = _xmlDocument.SelectSingleNode(@"@groupid");

                if (enabledAttribute != null)
                {
                    _panelCreate.Enabled = (enabledAttribute.Value.ToLower() == "true") ? true : false;
                }

                if (visibleAttribute != null)
                {
                    _panelCreate.Visible = (visibleAttribute.Value.ToLower() == "true") ? true : false;
                }

                if (layoutType != null)
                {
                    if (layoutType.Value.ToLower() == "stack-horizontal")
                    {
                        _panelCreate.PanelLayoutType = LayoutType.StackHorizontal;
                    }
                    else if (layoutType.Value.ToLower() == "stack-vertical")
                    {
                        _panelCreate.PanelLayoutType = LayoutType.StackVertical;
                    }
                }

                if (groupId != null)
                {
                    _panelCreate.UserInterface.AddPanelToGroup(_panelCreate, groupId.Value);
                }
            }
            else
            {
                throw new Exception("Panel Initialization Failed: No ID attribute was found.");
            }
        }


        protected virtual void LoadEventHandlers()
        {
            XmlNode onHoverAttribute = _xmlDocument.SelectSingleNode(@"@onHover");
            XmlNode onClickAttribute = _xmlDocument.SelectSingleNode(@"@onClick");
            XmlNode onFocusAttribute = _xmlDocument.SelectSingleNode(@"@onFocus");
            XmlNode onLostFocusAttribute = _xmlDocument.SelectSingleNode(@"@onLostFocus");

            if (onHoverAttribute != null)
            {
                _panelCreate.Hover += _panelCreate.UserInterface.GetInheritedEventHandler(onHoverAttribute.Value);
            }

            if (onClickAttribute != null)
            {
                _panelCreate.Click += _panelCreate.UserInterface.GetInheritedEventHandler(onClickAttribute.Value);
            }

            if (onFocusAttribute != null)
            {
                _panelCreate.Focus += _panelCreate.UserInterface.GetInheritedEventHandler(onFocusAttribute.Value);
            }

            if (onLostFocusAttribute != null)
            {
                _panelCreate.LostFocus += _panelCreate.UserInterface.GetInheritedEventHandler(onLostFocusAttribute.Value);
            }
        }

        protected virtual void LoadDesignElements()
        {
            if (_panelCreate.BackdropTextures != null)
            {
                _panelCreate.BackdropTextures.Clear();
            }
            else
            {
                _panelCreate.BackdropTextures = new Dictionary<string, UIBackdropTexture>();
            }

            if (_panelCreate.Textures != null)
            {
                _panelCreate.Textures.Clear();
            }
            else
            {
                _panelCreate.Textures = new Dictionary<string, Texture2D>();
            }

            if (_panelCreate.Colors != null)
            {
                _panelCreate.Colors.Clear();
            }
            else
            {
                _panelCreate.Colors = new Dictionary<string, Color>();
            }

            if (_panelCreate.Fonts != null)
            {
                _panelCreate.Fonts.Clear();
            }
            else
            {
                _panelCreate.Fonts = new Dictionary<string, SpriteFont>();
            }

            XmlNodeList backdropTextureNodes = _xmlDocument.SelectNodes(@"BackdropTextures/BackdropTexture");

            if (backdropTextureNodes != null)
                foreach (XmlNode backdropTextureNode in backdropTextureNodes)
                {
                    LoadBackdropTexture(backdropTextureNode);
                }

            XmlNodeList textureNodes = _xmlDocument.SelectNodes(@"Textures/Texture");

            if (textureNodes != null)
                foreach (XmlNode textureNode in textureNodes)
                {
                    LoadTexture(textureNode);
                }

            XmlNodeList colorNodes = _xmlDocument.SelectNodes(@"Colors/Color");

            if (colorNodes != null)
                foreach (XmlNode colorNode in colorNodes)
                {
                    LoadColor(colorNode);
                }

            XmlNodeList fontNodes = _xmlDocument.SelectNodes(@"Fonts/Font");

            if (fontNodes != null)
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
                XmlNode activeAttribute = backdropTextureNode.SelectSingleNode(@"@active");

                string name = (nameAttribute != null) ? nameAttribute.Value : "";
                string texture = (textureAttribute != null) ? textureAttribute.Value : "";

                int tileWidth = 0;
                int tileHeight = 0;

                bool active = false;

                if (tileWidthAttribute != null)
                {
                    Int32.TryParse(tileWidthAttribute.Value, out tileWidth);
                }

                if (tileHeightAttribute != null)
                {
                    Int32.TryParse(tileWidthAttribute.Value, out tileHeight);
                }

                if (activeAttribute != null)
                {
                    active = (activeAttribute.Value.ToLower() == "true") ? true : false;
                }

                if (name.Length > 0 && texture.Length > 0 && tileWidth > 0 && tileHeight > 0)
                {
                    try
                    {
                        Texture2D backgroundTexture = _panelCreate.UserInterface.Game.Content.Load<Texture2D>(texture);
                        UIBackdropTexture backdropTexture = new UIBackdropTexture(backgroundTexture, tileWidth, tileHeight);

                        if (!_panelCreate.BackdropTextures.Keys.Contains(name))
                        {
                            _panelCreate.BackdropTextures.Add(name, backdropTexture);
                        }

                        if (active)
                        {
                            _panelCreate.BackdropTexture = backdropTexture;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Texture load failed for the following texture: " + texture, ex);
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
                        Texture2D texture = _panelCreate.UserInterface.Game.Content.Load<Texture2D>(asset);

                        if (!_panelCreate.Textures.Keys.Contains(name))
                        {
                            _panelCreate.Textures.Add(name, texture);
                        }

                        if (active)
                        {
                            _panelCreate.Texture = texture;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Texture load failed for the following texture: " + asset, ex);
                    }
                }
            }
        }

        protected void LoadColor(XmlNode colorNode)
        {
            if (colorNode != null)
            {
                XmlNode nameAttribute = colorNode.SelectSingleNode(@"@name");
                XmlNode activeAttribute = colorNode.SelectSingleNode(@"@active");

                string name = (nameAttribute != null) ? nameAttribute.Value : "";
                bool active = false;

                if (activeAttribute != null)
                {
                    active = (activeAttribute.Value.ToLower() == "true") ? true : false;
                }

                if (name.Length > 0 && colorNode.FirstChild != null)
                {
                    try
                    {
                        string htmlColorString = colorNode.FirstChild.Value;
                        Color color = Common.StringToColor(htmlColorString);

                        if (!_panelCreate.Colors.Keys.Contains(name))
                        {
                            _panelCreate.Colors.Add(name, color);
                        }

                        if (active)
                        {
                            _panelCreate.Color = color;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Could not load color.", ex);
                    }
                }
            }
        }

        protected virtual void LoadFont(XmlNode fontNode)
        {
            if (fontNode != null)
            {
                XmlNode nameAttribute = fontNode.SelectSingleNode(@"@name");
                XmlNode activeAttribute = fontNode.SelectSingleNode(@"@active");

                string name = (nameAttribute != null) ? nameAttribute.Value : "";
                bool active = false;

                if (activeAttribute != null)
                {
                    active = (activeAttribute.Value.ToLower() == "true") ? true : false;
                }

                if (name.Length > 0 && fontNode.FirstChild != null)
                {
                    try
                    {
                        string assetName = fontNode.FirstChild.Value;
                        SpriteFont font = _panelCreate.UserInterface.Game.Content.Load<SpriteFont>(assetName);

                        if (!_panelCreate.Fonts.Keys.Contains(name))
                        {
                            _panelCreate.Fonts.Add(name, font);
                        }

                        if (active)
                        {
                            _panelCreate.Font = font;
                        }
                    }
                    catch
                    {
                        throw new Exception("Can not load font.");
                    }
                }
            }
        }


        protected virtual void LoadDrawRectangle()
        {
            string position = "relative";

            int dx = 0;
            int dy = 0;
            int dw = 0;
            int dh = 0;

            XmlNode positionAttribute = _xmlDocument.SelectSingleNode(@"@position");

            if (positionAttribute != null)
            {
                position = positionAttribute.Value.ToLower();

                XmlNode xAttribute = _xmlDocument.SelectSingleNode(@"@x");
                XmlNode yAttribute = _xmlDocument.SelectSingleNode(@"@y");
                XmlNode widthAttribute = _xmlDocument.SelectSingleNode(@"@width");
                XmlNode heightAttribute = _xmlDocument.SelectSingleNode(@"@height");

                if (xAttribute != null && yAttribute != null && widthAttribute != null && heightAttribute != null)
                {
                    dx = GetIntegerValue(xAttribute.Value);
                    dy = GetIntegerValue(yAttribute.Value);
                    dw = GetIntegerValue(widthAttribute.Value);
                    dh = GetIntegerValue(heightAttribute.Value);
                }
            }
            if (_panelCreate.Parent != null)
            {
                dx += _panelCreate.Parent.LayoutCurrentPosition.X;
                dy += _panelCreate.Parent.LayoutCurrentPosition.Y;
            }
            _panelCreate.PosRect = new Rectangle(dx, dy, dw, dh);
            _panelCreate.DrawRectangle = CalculateDrawRectangle(position, dx, dy, dw, dh, _panelCreate.Parent,
                                                                _panelCreate.UserInterface);
        }


        public static Rectangle CalculateDrawRectangle(string position, int px, int py, int pw, int ph, Panel parent, UserInterface ui)
        {
            int dx = px;
            int dy = py;
            int dw = pw;
            int dh = ph;

            int rx = 0;
            int ry = 0;
            int rw = 0;
            int rh = 0;

            if (parent == null || position == "absolute")
            {
                rx = 0;
                ry = 0;
                rw = ui.Game.GraphicsDevice.Viewport.Width;
                rh = ui.Game.GraphicsDevice.Viewport.Height;
            }
            else
            {
                rx = parent.DrawRectangle.X;
                ry = parent.DrawRectangle.Y;
                rw = parent.DrawRectangle.Width;
                rh = parent.DrawRectangle.Height;
            }

            switch (position)
            {
                case "top":
                    dx += rx + (int)Math.Floor(rw / 2.00);
                    dy += ry;
                    dx -= (int)Math.Floor(dw / 2.00);
                    break;
                case "top-left":
                    dx += rx;
                    dy += ry;
                    break;
                case "top-right":
                    dx += rx + rw;
                    dy += ry;
                    dx -= dw;
                    break;
                case "center":
                    dx += rx + (int)Math.Floor(rw / 2.00);
                    dy += ry + (int)Math.Floor(rh / 2.00);
                    dx -= (int)Math.Floor(dw / 2.00);
                    dy -= (int)Math.Floor(dh / 2.00);
                    break;
                case "center-left":
                    dx += rx;
                    dy += ry + (int)Math.Floor(rh / 2.00);
                    dy -= (int)Math.Floor(dh / 2.00);
                    break;
                case "center-right":
                    dx += rx + rw;
                    dy += ry + (int)Math.Floor(rh / 2.00);
                    dx -= dw;
                    dy -= (int)Math.Floor(dh / 2.00);
                    break;
                case "bottom":
                    dx += rx + (int)Math.Floor(rw / 2.00);
                    dy += ry + rh;
                    dx -= (int)Math.Floor(dw / 2.00);
                    dy -= dh;
                    break;
                case "bottom-left":
                    dx += rx;
                    dy += ry + rh;
                    dy -= dh;
                    break;
                case "bottom-right":
                    dx += rx + rw;
                    dy += ry + rh;
                    dx -= dw;
                    dy -= dh;
                    break;
                default:
                    break;
            }

            return new Rectangle(dx, dy, dw, dh);
        }

        protected virtual void LoadPanels()
        {
            XmlNodeList panelNodes = _xmlDocument.SelectNodes(@"Panels/Panel");

            if (panelNodes != null)
                foreach (XmlNode panelNode in panelNodes)
                {
                    XmlNode idNode = panelNode.SelectSingleNode(@"@id");
                    XmlNode inheritsAttribute = panelNode.SelectSingleNode(@"@type");

                    if (idNode != null)
                    {
                        string id = idNode.Value;

                        if (inheritsAttribute != null)
                        {
                            string inheritedClassName = inheritsAttribute.Value;

                            _panelCreate.AddChildPanel(GetInheritedPanel(inheritedClassName, panelNode, _panelCreate.UserInterface));
                        }
                        else
                        {
                            _panelCreate.AddChildPanel(new PanelFactory().CreateNewPanelByTypeWithParent(panelNode, _panelCreate.UserInterface, typeof(Panel) ,_panelCreate));
                        }
                    }
                }
        }

        public Panel GetInheritedPanel(string inheritedClassName, XmlNode panelXML, UserInterface userInterface)
        {
            Panel panel;

            if (InterfaceFactory.ReferencedTypes.Keys.Contains(inheritedClassName))
            {
                panel = new PanelFactory().CreateNewPanelByTypeWithParent(panelXML, _panelCreate.UserInterface,
                                                                          InterfaceFactory.ReferencedTypes[
                                                                              inheritedClassName],
                                                                          _panelCreate);
            }
            else
            {
                throw new Exception("Get Inherited Panel Failed: Could not find class of type:" + inheritedClassName);
            }

            return panel;
        }

        public int GetIntegerValue(string value)
        {
            if (_panelCreate.UserInterface.XmlSetVariables.ContainsKey(value))
            {
                return _panelCreate.UserInterface.XmlSetVariables[value];
            }
            return Int32.Parse(value);
        }
    }
}
