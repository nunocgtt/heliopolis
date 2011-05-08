using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heliopolis.UILibrary
{
    public class UIPanel_InputBox : Panel, IPanel
    {
        public string Value { get; set; }

        public event EventHandler BackspaceKeyPressed;
        public event EventHandler EnterKeyPressed;
        public event EventHandler TabKeyPressed;

        protected string valueAlignment;
        protected bool cursorToggle;
        protected DateTime cursorLastToggled;

        protected void LoadValue(XmlNode xmlNode)
        {
            Value = "";
            valueAlignment = "center-left";

            XmlNode valueNode = xmlNode.SelectSingleNode(@"Value");

            if (valueNode != null)
            {
                if (valueNode.FirstChild != null)
                {
                    Value = valueNode.FirstChild.Value;
                }

                XmlNode alignmentAttribute = valueNode.SelectSingleNode(@"@alignment");

                if (alignmentAttribute != null)
                {
                    this.valueAlignment = alignmentAttribute.Value;
                }
            }
        }

        public override void Enable()
        {
            base.Enable();

            base.BackdropTexture = base.GetBackdropTexture("InputBox-Backdrop");

            Color = base.GetColor("InputBox-TextColor");
            Font = base.GetFont("InputBox-Font");
        }

        public override void Disable()
        {
            base.Disable();

            base.BackdropTexture = base.GetBackdropTexture("InputBox-Disabled-Backdrop");

            Color = base.GetColor("InputBox-Disabled-TextColor");
            Font = base.GetFont("InputBox-Font");

/*            if (this.keyboardInput != null)
            {
                this.keyboardInput.Dispose();
                this.keyboardInput = null;
            }*/
        }

        public override void Hide()
        {
            base.Hide();

/*            if (this.keyboardInput != null)
            {
                this.keyboardInput.Dispose();
                this.keyboardInput = null;
            }*/
        }

        public override bool Update()
        {
            bool keepGoing = base.Update();
            
            //if (base.Enabled)
            //{
            //    if (!base.HasFocus && this.keyboardInput != null)
            //    {
            //        this.keyboardInput.Dispose();
            //        this.keyboardInput = null;
            //    }
            //    else if (base.HasFocus && this.keyboardInput == null)
            //    {
            //        this.keyboardInput = new UIKeyboardInput(base.UserInterface.Game.Window.Handle);

            //        this.cursorToggle = true;
            //        this.cursorLastToggled = DateTime.Now;
            //    }

            //    if (base.HasFocus && this.keyboardInput != null)
            //    {
            //        if (this.keyboardInput.BackSpacePressed && Value.Length > 0)
            //        {
            //            Value = Value.Remove(Value.Length - 1, 1);

            //            if (BackspaceKeyPressed != null)
            //            {
            //                BackspaceKeyPressed(this, EventArgs.Empty);
            //                keepGoing = false;
            //            }
            //        }
            //        else if (this.keyboardInput.EnterKeyPressed)
            //        {
            //            if (EnterKeyPressed != null)
            //            {
            //                EnterKeyPressed(this, EventArgs.Empty);
            //                keepGoing = false;
            //            }
            //        }
            //        else if (this.keyboardInput.TabPressed)
            //        {
            //            if (TabKeyPressed != null)
            //            {
            //                TabKeyPressed(this, EventArgs.Empty);
            //                keepGoing = false;
            //            }
            //        }
            //        else
            //        {
            //            Value += this.keyboardInput.InputBuffer;
            //            this.keyboardInput.ClearBuffer();
            //        }
            //    }
            //}

            return keepGoing;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (base.Visible)
            {
                if (Value.Length > 0 || base.HasFocus)
                {
                    Rectangle textRectangle = new Rectangle();

                    textRectangle.X = base.DrawRectangle.X + BackdropTexture.TileWidth;
                    textRectangle.Y = base.DrawRectangle.Y + base.BackdropTexture.TileHeight;
                    textRectangle.Width = base.DrawRectangle.Width - (base.BackdropTexture.TileWidth * 2);
                    textRectangle.Height = base.DrawRectangle.Height - (base.BackdropTexture.TileHeight * 2);

                    Vector2 fontDimensions;

                    if (Value.Length == 0)
                    {
                        fontDimensions = Font.MeasureString("M");
                        fontDimensions.X = 0;
                    }
                    else
                    {
                        fontDimensions = Font.MeasureString(Value);
                    }

                    int tw = (int)fontDimensions.X;
                    int th = (int)fontDimensions.Y;

                    int dx = textRectangle.X;
                    int dy = textRectangle.Y;

                    switch (this.valueAlignment)
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

                    if (base.HasFocus)
                    {
                        if (DateTime.Now.Subtract(cursorLastToggled).Milliseconds > 500)
                        {
                            cursorToggle = !cursorToggle;
                            cursorLastToggled = DateTime.Now;
                        }
                    }

                    Rectangle oldScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;

                    spriteBatch.End();
                    spriteBatch.GraphicsDevice.ScissorRectangle = textRectangle;
                    spriteBatch.Begin();

                    spriteBatch.DrawString(Font, Value, new Vector2(dx, dy), Color);

                    if (base.HasFocus && this.cursorToggle)
                    {
                        Texture2D cursorTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, th + 4, true, SurfaceFormat.Color);

                        Color[] textureData = new Color[th + 4];

                        for (int i = 0; i < textureData.Length; i++)
                        {
                            textureData[i] = Color;
                        }

                        cursorTexture.SetData(textureData);

                        spriteBatch.Draw(cursorTexture, new Rectangle(dx + tw + 2, dy - 2, 1, th + 4), Color.White);
                    }

                    spriteBatch.End();
                    spriteBatch.GraphicsDevice.ScissorRectangle = oldScissorRectangle;
                    spriteBatch.Begin();
                }
            }
        }

        public void LoadEventHandlers(XmlNode xmlNode)
        {
            XmlNode onBackspaceKeyPressedAttribute = xmlNode.SelectSingleNode(@"@onBackspaceKeyPressed");
            XmlNode onEnterKeyPressedAttribute = xmlNode.SelectSingleNode(@"@onEnterKeyPressed");
            XmlNode onTabKeyPressedAttribute = xmlNode.SelectSingleNode(@"@onTabKeyPressed");

            if (onBackspaceKeyPressedAttribute != null)
            {
                BackspaceKeyPressed += UserInterface.GetInheritedEventHandler(onBackspaceKeyPressedAttribute.Value);
            }

            if (onEnterKeyPressedAttribute != null)
            {
                EnterKeyPressed += UserInterface.GetInheritedEventHandler(onEnterKeyPressedAttribute.Value);
            }

            if (onTabKeyPressedAttribute != null)
            {
                TabKeyPressed += UserInterface.GetInheritedEventHandler(onTabKeyPressedAttribute.Value);
            }
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
            EnableClipping = false; // Force clipping off, because we need to manual clip later. Expensive to double clip.
            //keyboardInput = null;
            cursorToggle = false;
            cursorLastToggled = DateTime.Now;

            LoadValue(xmlNode);
        }

        public void LoadDrawRectangle(XmlNode xmlNode)
        {
            
        }

    }
}