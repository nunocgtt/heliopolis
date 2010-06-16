using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Heliopolis.UILibrary
{
    public enum LayoutType
    {
        Absolute,
        StackHorizontal,
        StackVertical
    }

    public class Panel
    {
        public event EventHandler Hover;
        public event EventHandler Click;
        public event EventHandler Focus;
        public event EventHandler LostFocus;
        
        protected string id;
        public string ID { get { return id; } set { id = value; } }
 
        public Panel Parent { get; set; }
        public UserInterface UserInterface { get; set; }

        public LayoutType PanelLayoutType { get; set; }
        public Point LayoutCurrentPosition;
        public Rectangle PosRect;
        public string GroupId { get; set; }

        public Dictionary<string, Panel> Panels { get; set; }

        public bool HasFocus { get; set; }

        public Rectangle DrawRectangle { get; set; }
        public bool EnableClipping { get; set; }

        public virtual UIBackdropTexture BackdropTexture { get; set; }
        public Dictionary<string, UIBackdropTexture> BackdropTextures { get; set; }

        public virtual Texture2D Texture { get; set; }
        public Dictionary<string, Texture2D> Textures { get; set; }

        public virtual Color Color { get; set; }
        public Dictionary<string, Color> Colors { get; set; }

        public virtual SpriteFont Font { get; set; }
        public Dictionary<string, SpriteFont> Fonts { get; set; }
        
        protected bool enabled = true;
        public bool Enabled
        {
            get { return this.enabled; }
            set {
                if (value)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
            }
        }

        protected bool visible = true;
        public bool Visible
        {
            get { return this.visible; }
            set
            {
                if (value)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }

        internal Panel()
        {
            LayoutCurrentPosition = new Point(0, 0);
        }

        public virtual void AddChildPanel(Panel panel)
        {
            if (Panels.Keys.Contains(panel.ID))
            {
                throw new Exception("UIPanel ID already in use: " + panel.ID + ". Parent UIPanel ID: " + ID);
            }
            Panels.Add(panel.ID, panel);
            switch (PanelLayoutType)
            {
                case LayoutType.StackHorizontal:
                    int addX = panel.PosRect.Left + panel.PosRect.Width;
                    LayoutCurrentPosition.X = addX;
                    break;
                case LayoutType.StackVertical:
                    int addY = panel.PosRect.Top + panel.PosRect.Height;
                    LayoutCurrentPosition.Y = addY;
                    break;
            }
        }

        public virtual Panel GetPanel(string panelID)
        {
            Panel panel = null;

            if (Panels.Keys.Contains(panelID))
            {
                panel = Panels[panelID];
            }

            return panel;
        }

        public virtual UIBackdropTexture GetBackdropTexture(string name)
        {
            UIBackdropTexture backdropTexture = null;

            if (BackdropTextures.Keys.Contains(name))
            {
                backdropTexture = BackdropTextures[name];
            }
            else
            {
                backdropTexture = UserInterface.Theme.GetBackdropTexture(name);
            }

            return backdropTexture;
        }

        public virtual Texture2D GetTexture(string name)
        {
            Texture2D texture = null;

            if (Textures.Keys.Contains(name))
            {
                texture = Textures[name];
            }
            else
            {
                texture = UserInterface.Theme.GetTexture(name);
            }

            return texture;
        }

        public virtual Color GetColor(string name)
        {
            Color color = Color.PaleGreen;

            if (Colors.Keys.Contains(name))
            {
                color = Colors[name];
            }
            else
            {
                color = UserInterface.Theme.GetColor(name);
            }

            return color;
        }

        public virtual SpriteFont GetFont(string name)
        {
            SpriteFont font = null;

            if (Fonts.Keys.Contains(name))
            {
                font = Fonts[name];
            }
            else
            {
                font = UserInterface.Theme.GetFont(name);
            }

            return font;
        }

        public virtual void Enable()
        {
            this.enabled = true;
        }

        public virtual void Disable()
        {
            this.enabled = false;
            
            HasFocus = false;
        }

        public virtual void Show()
        {
            this.visible = true;
        }

        public virtual void Hide()
        {
            this.visible = false;

            HasFocus = false;
        }

        public virtual bool Update()
        {
            bool keepGoing = true;

            if (visible)
            {
                foreach (Panel panel in Panels.Values)
                {
                    keepGoing = panel.Update();

                    if (!keepGoing)
                    {
                        break;
                    }
                }

                if (keepGoing && Enabled)
                {
                    int lx = DrawRectangle.X;
                    int ly = DrawRectangle.Y;
                    int lw = DrawRectangle.Width;
                    int lh = DrawRectangle.Height;

                    int mx = UserInterface.CurrentMouseState.X;
                    int my = UserInterface.CurrentMouseState.Y;

                    if (mx >= lx && my >= ly && mx <= lx + lw && my <= ly + lh)
                    {
                        if (Hover != null)
                        {
                            Hover(this, EventArgs.Empty);
                            keepGoing = false;
                        }

                        if (UserInterface.CurrentMouseState.LeftButton != ButtonState.Pressed && UserInterface.PreviousMouseState.LeftButton == ButtonState.Pressed)
                        {
                            if (Click != null)
                            {
                                Click(this, EventArgs.Empty);
                            }

                            if (UserInterface.Focus != this)
                            {
                                UserInterface.Focus = this;
                                HasFocus = true;

                                if (Focus != null)
                                {
                                    Focus(this, EventArgs.Empty);
                                }
                            }

                            keepGoing = false;
                        }
                    }

                    if (HasFocus && UserInterface.Focus != this)
                    {
                        HasFocus = false;

                        if (LostFocus != null)
                        {
                            LostFocus(this, EventArgs.Empty);
                        }
                    }
                }
            }
            return keepGoing;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
/*
                Rectangle oldScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;

                if (EnableClipping)
                {
                    spriteBatch.End();
                    spriteBatch.GraphicsDevice.ScissorRectangle = DrawRectangle;
                    spriteBatch.Begin();
                }
*/
                if (BackdropTexture != null)
                {
                    DrawBackdrop(spriteBatch);
                }

                foreach (Panel panel in Panels.Values)
                {
                    panel.Draw(spriteBatch);
                }

/*                if (EnableClipping)
                {
                    spriteBatch.End();
                    spriteBatch.GraphicsDevice.ScissorRectangle = oldScissorRectangle;
                    spriteBatch.Begin();
                }*/
            }
        }

        protected virtual void DrawBackdrop(SpriteBatch spriteBatch)
        {
            int dx = DrawRectangle.X;
            int dy = DrawRectangle.Y;
            int dw = DrawRectangle.Width;
            int dh = DrawRectangle.Height;

            int tx = dx;
            int ty = dy;
            int tw = BackdropTexture.TileWidth;
            int th = BackdropTexture.TileHeight;

            // correct dimensions to be a multiple of the texture dimensions...
            dw = tw * (int)Math.Floor((double)(dw / tw));
            dh = th * (int)Math.Floor((double)(dh / th));

            while (ty < dy + dh)
            {
                if (ty != dy && ty != dy + dh - th)
                {
                    spriteBatch.Draw(BackdropTexture.Texture, new Rectangle(dx, ty, tw, th), new Rectangle(0, th, tw, th), Color.White);
                    spriteBatch.Draw(BackdropTexture.Texture, new Rectangle(dx + dw - tw, ty, tw, th), new Rectangle(tw * 2, th, tw, th), Color.White);
                }

                tx = dx + tw;

                while (tx < dx + dw - tw)
                {
                    if (ty == dy)
                    {
                        spriteBatch.Draw(BackdropTexture.Texture, new Rectangle(tx, dy, tw, th), new Rectangle(tw, 0, tw, th), Color.White);
                    }
                    else if (ty == dy + dh - th)
                    {
                        spriteBatch.Draw(BackdropTexture.Texture, new Rectangle(tx, dy + dh - th, tw, th), new Rectangle(tw, th * 2, tw, th), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(BackdropTexture.Texture, new Rectangle(tx, ty, tw, th), new Rectangle(tw, th, tw, th), Color.White);
                    }

                    tx += tw;
                }

                ty += th;
            }

            spriteBatch.Draw(BackdropTexture.Texture, new Rectangle(dx, dy, tw, th), new Rectangle(0, 0, tw, th), Color.White);
            spriteBatch.Draw(BackdropTexture.Texture, new Rectangle(dx + dw - tw, dy, tw, th), new Rectangle(tw * 2, 0, tw, th), Color.White);
            spriteBatch.Draw(BackdropTexture.Texture, new Rectangle(dx, dy + dh - th, tw, th), new Rectangle(0, th * 2, tw, th), Color.White);
            spriteBatch.Draw(BackdropTexture.Texture, new Rectangle(dx + dw - tw, dy + dh - th, tw, th), new Rectangle(tw * 2, th * 2, tw, th), Color.White);
        }
    }
}
