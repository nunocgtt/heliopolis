using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Heliopolis.UILibrary
{
    public class UIPanel_Button : UIPanel, IPanel
    {
        public string Text { get; set; }
        public string ModifyGroupVisibility { get; set; }
        public string PanelToShow { get; set; }

        protected string textAlignment;

        protected bool buttonDepressed;

        protected void LoadText(XmlNode xmlNode)
        {
            Text = "";
            textAlignment = "center";

            XmlNode textNode = xmlNode.SelectSingleNode(@"Text");
            XmlNode modGroupVisiblily = xmlNode.SelectSingleNode("@modifyGroupVisiblity");
            XmlNode panelToShow = xmlNode.SelectSingleNode("@panelToShow");

            if (textNode != null)
            {
                if (textNode.FirstChild != null)
                {
                    Text = textNode.FirstChild.Value;
                }

                XmlNode alignmentAttribute = textNode.SelectSingleNode(@"@alignment");

                if (alignmentAttribute != null)
                {
                    this.textAlignment = alignmentAttribute.Value;
                }
            }

            if (modGroupVisiblily != null && panelToShow != null)
            {
                ModifyGroupVisibility = modGroupVisiblily.Value;
                PanelToShow = panelToShow.Value;
            }
        }

        public override void Enable()
        {
            base.Enable();

            base.BackdropTexture = base.GetBackdropTexture("Button-Backdrop");

            Color = base.GetColor("Button-TextColor");
            Font = base.GetFont("Button-Font");
        }

        public override void Disable()
        {
            base.Disable();

            base.BackdropTexture = base.GetBackdropTexture("Button-Disabled-Backdrop");

            Color = base.GetColor("Button-Disabled-TextColor");
            Font = base.GetFont("Button-Font");
        }

        public override bool Update()
        {
            if (visible)
            {
                int lx = DrawRectangle.X;
                int ly = DrawRectangle.Y;
                int lw = DrawRectangle.Width;
                int lh = DrawRectangle.Height;

                int mx = UserInterface.CurrentMouseState.X;
                int my = UserInterface.CurrentMouseState.Y;


                if (mx >= lx && my >= ly && mx <= lx + lw && my <= ly + lh)
                {
                    if (UserInterface.CurrentMouseState.LeftButton == ButtonState.Pressed && UserInterface.PreviousMouseState.LeftButton != ButtonState.Pressed)
                    {
                        base.BackdropTexture = base.GetBackdropTexture("Button-Pressed-Backdrop");

                        Color = base.GetColor("Button-Pressed-TextColor");

                        buttonDepressed = true;
                    }
                }

                if (UserInterface.CurrentMouseState.LeftButton != ButtonState.Pressed && UserInterface.PreviousMouseState.LeftButton == ButtonState.Pressed && buttonDepressed)
                {
                    base.BackdropTexture = base.GetBackdropTexture("Button-Backdrop");

                    Color = base.GetColor("Button-TextColor");

                    buttonDepressed = false;

                    ProcessPanelVisibilty();
                }
            }

            return base.Update();
        }

        private void ProcessPanelVisibilty()
        {
            if (!string.IsNullOrEmpty(ModifyGroupVisibility) && !string.IsNullOrEmpty(PanelToShow))
            {
                UserInterface.SetPanelGroupVisibilty(ModifyGroupVisibility, PanelToShow);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (base.Visible)
            {
                if (Text.Length > 0)
                {
                    Rectangle textRectangle = new Rectangle();

                    textRectangle.X = base.DrawRectangle.X + BackdropTexture.TileWidth;
                    textRectangle.Y = base.DrawRectangle.Y + base.BackdropTexture.TileHeight;
                    textRectangle.Width = base.DrawRectangle.Width - (base.BackdropTexture.TileWidth * 2);
                    textRectangle.Height = base.DrawRectangle.Height - (base.BackdropTexture.TileHeight * 2);

                    Vector2 fontDimensions;

                    if (Text.Length == 0)
                    {
                        fontDimensions = Font.MeasureString("M");
                        fontDimensions.X = 0;
                    }
                    else
                    {
                        fontDimensions = Font.MeasureString(Text);
                    }

                    int tw = (int)fontDimensions.X;
                    int th = (int)fontDimensions.Y;

                    int dx = textRectangle.X;
                    int dy = textRectangle.Y;

                    switch (this.textAlignment)
                    {
                        case "top":
                            dx += (int)Math.Floor(textRectangle.Width / 2.00);
                            dx -= (int)Math.Floor(tw / 2.00);
                            break;
                        case "top-left":
                            break;
                        case "top-right":
                            dx += textRectangle.Width;
                            dx -= tw;
                            break;
                        case "center":
                            dx += (int)Math.Floor(textRectangle.Width / 2.00);
                            dy += (int)Math.Floor(textRectangle.Height / 2.00);
                            dx -= (int)Math.Floor(tw / 2.00);
                            dy -= (int)Math.Floor(th / 2.00);
                            break;
                        case "center-left":
                            dy += (int)Math.Floor(textRectangle.Height / 2.00);
                            dy -= (int)Math.Floor(th / 2.00);
                            break;
                        case "center-right":
                            dx += textRectangle.Width;
                            dy += (int)Math.Floor(textRectangle.Height / 2.00);
                            dx -= tw;
                            dy -= (int)Math.Floor(th / 2.00);
                            break;
                        case "bottom":
                            dx += (int)Math.Floor(textRectangle.Width / 2.00);
                            dy += textRectangle.Height;
                            dx -= (int)Math.Floor(tw / 2.00);
                            dy -= th;
                            break;
                        case "bottom-left":
                            dy += textRectangle.Height;
                            dy -= th;
                            break;
                        case "bottom-right":
                            dx += textRectangle.Width;
                            dy += textRectangle.Height;
                            dx -= tw;
                            dy -= th;
                            break;
                        default:
                            break;
                    }

                    spriteBatch.DrawString(Font, Text, new Vector2(dx, dy), Color);
                }
            }
        }

        #region IPanel Members

        public void LoadEventHandlers(XmlNode xmlNode)
        {
            
        }

        public void LoadDesignElements()
        {
            if (base.Enabled)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        public void Load(XmlNode xmlNode)
        {
            buttonDepressed = false;

            LoadText(xmlNode);
        }

        public void LoadDrawRectangle(XmlNode xmlNode)
        {
            
        }

        #endregion
    }
}