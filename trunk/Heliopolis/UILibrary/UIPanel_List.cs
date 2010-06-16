using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Heliopolis.UILibrary
{
    public class UIPanel_List : Panel, IPanel
    {
        public List<string> ListItems;

        protected int selectedIndex;
        public int SelectedIndex
        {
            get { return this.selectedIndex; }
            set { SelectedItem(value); }
        }

        protected int firstDisplayedIndex;
        protected int maxDisplayableItems;

        protected Texture2D rowBackgroundTexture;
        protected Texture2D rowAltBackgroundTexture;
        protected Texture2D rowSelectedBackgroundTexture;

        protected Texture2D upButtonTexture;
        protected Texture2D downButtonTexture;

        protected Color rowTextColor;
        protected Color rowAltTextColor;
        protected Color rowSelectedTextColor;

        protected Rectangle itemDisplayRectangle;

        protected bool upButtonDepressed;
        protected bool downButtonDepressed;

        protected void LoadFromXML(XmlNode loadFrom)
        {
            ListItems = new List<string>();

            selectedIndex = 0;
            firstDisplayedIndex = 0;

            XmlNodeList listItemNodes = loadFrom.SelectNodes("ListItems/ListItem");

            foreach (XmlNode listItemNode in listItemNodes)
            {
                if (listItemNode.FirstChild != null)
                {
                    ListItems.Add(listItemNode.FirstChild.Value);
                }

                bool selected = false;

                XmlNode selectedAttribute = listItemNode.SelectSingleNode(@"@selected");

                if (selectedAttribute != null)
                {
                    selected = (selectedAttribute.Value.ToLower() == "true") ? true : false;
                }

                if (selected)
                {
                    selectedIndex = ListItems.Count - 1;
                    firstDisplayedIndex = selectedIndex;
                }
            }
        }

        protected void BuildRowBackgroundTextures()
        {
            int rw = itemDisplayRectangle.Width;
            int rh = (int)base.Font.MeasureString("M").Y + 4;

            Color rowBackgroundColor = base.GetColor("List-RowBackgroundColor");
            Color rowAltBackgroundColor = base.GetColor("List-RowAltBackgroundColor");
            Color rowSelectedBackgroundColor = base.GetColor("List-RowSelectedBackgroundColor");

            Color[] rowBackgroundData = new Color[rw * rh];
            Color[] rowAltBackgroundData = new Color[rw * rh];
            Color[] rowSelectedBackgroundData = new Color[rw * rh];

            rowBackgroundTexture = new Texture2D(base.UserInterface.Game.GraphicsDevice, rw, rh, 1, TextureUsage.None, SurfaceFormat.Color);
            rowAltBackgroundTexture = new Texture2D(base.UserInterface.Game.GraphicsDevice, rw, rh, 1, TextureUsage.None, SurfaceFormat.Color);
            rowSelectedBackgroundTexture = new Texture2D(base.UserInterface.Game.GraphicsDevice, rw, rh, 1, TextureUsage.None, SurfaceFormat.Color);

            for (int i = 0; i < rowBackgroundData.Length; i++)
            {
                rowBackgroundData[i] = rowBackgroundColor;
                rowAltBackgroundData[i] = rowAltBackgroundColor;
                rowSelectedBackgroundData[i] = rowSelectedBackgroundColor;
            }

            rowBackgroundTexture.SetData(rowBackgroundData);
            rowAltBackgroundTexture.SetData(rowAltBackgroundData);
            rowSelectedBackgroundTexture.SetData(rowSelectedBackgroundData);
        }

        public override bool Update()
        {
            bool keepGoing = base.Update();

            if (base.Visible && base.Enabled)
            {
                int lx = base.DrawRectangle.X;
                int ly = base.DrawRectangle.Y;
                int lw = base.DrawRectangle.Width;
                int lh = base.DrawRectangle.Height;

                int mx = base.UserInterface.CurrentMouseState.X;
                int my = base.UserInterface.CurrentMouseState.Y;

                if (mx >= lx && my >= ly && mx <= lx + lw && my <= ly + lh)
                {
                    int ix = this.itemDisplayRectangle.X;
                    int iy = this.itemDisplayRectangle.Y;
                    int iw = this.itemDisplayRectangle.Width;
                    int ih = this.itemDisplayRectangle.Height;

                    int ubx = lx + lw - this.upButtonTexture.Width;
                    int uby = ly;
                    int ubw = this.upButtonTexture.Width;
                    int ubh = this.upButtonTexture.Height;

                    int dbx = lx + lw - this.downButtonTexture.Width ;
                    int dby = ly + lh - this.downButtonTexture.Height;
                    int dbw = this.downButtonTexture.Width;
                    int dbh = this.downButtonTexture.Height;

                    bool overItemRectangle = (mx >= ix && my >= iy && mx <= ix + iw && my <= iy + ih);
                    bool overUpButton = (mx >= ubx && my >= uby && mx <= ubx + ubw && my <= uby + ubh);
                    bool overDownButton = (mx >= dbx && my >= dby && mx <= dbx + dbw && my <= dby + dbh);

                    if (UserInterface.CurrentMouseState.LeftButton == ButtonState.Pressed && UserInterface.PreviousMouseState.LeftButton != ButtonState.Pressed)
                    {
                        if (overUpButton)
                        {
                            this.upButtonTexture = base.GetTexture("List-UpButton-Pressed");
                            upButtonDepressed = true;
                        }
                        else if (overDownButton)
                        {
                            this.downButtonTexture = base.GetTexture("List-DownButton-Pressed");
                            downButtonDepressed = true;
                        }
                    }

                    if (base.UserInterface.CurrentMouseState.LeftButton != ButtonState.Pressed && base.UserInterface.PreviousMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (overItemRectangle)
                        {
                            int item = (int)Math.Floor((double)(my - ly) / (double)this.rowBackgroundTexture.Height);

                            this.selectedIndex = item + this.firstDisplayedIndex;

                            if (this.selectedIndex >= ListItems.Count)
                            {
                                this.selectedIndex = ListItems.Count - 1;
                            }
                        }
                        else if (overUpButton)
                        {
                            firstDisplayedIndex -= 1;
                            firstDisplayedIndex = (firstDisplayedIndex < 0) ? 0 : firstDisplayedIndex;
                        }
                        else if (overDownButton)
                        {
                            firstDisplayedIndex += 1;
                            firstDisplayedIndex = (firstDisplayedIndex >= ListItems.Count) ? ListItems.Count - 1 : firstDisplayedIndex;
                        }
                    }
                }

                if (UserInterface.CurrentMouseState.LeftButton != ButtonState.Pressed && (upButtonDepressed || downButtonDepressed))
                {
                    if (upButtonDepressed)
                    {
                        this.upButtonTexture = base.GetTexture("List-UpButton");
                        upButtonDepressed = false;
                    }
                    
                    if (downButtonDepressed)
                    {
                        this.downButtonTexture = base.GetTexture("List-DownButton");
                        downButtonDepressed = false;
                    }
                }
            }

            return keepGoing;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Rectangle oldScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = this.itemDisplayRectangle;
            spriteBatch.Begin();

            int dx = itemDisplayRectangle.X;
            int dy = itemDisplayRectangle.Y;

            this.firstDisplayedIndex = (this.firstDisplayedIndex >= ListItems.Count) ? ListItems.Count - 1 : this.firstDisplayedIndex;

            if (ListItems.Count - this.firstDisplayedIndex + 1 < this.maxDisplayableItems)
            {
                this.firstDisplayedIndex = ListItems.Count - this.maxDisplayableItems + 1;
            }

            this.firstDisplayedIndex = (this.firstDisplayedIndex <= 0) ? 0 : this.firstDisplayedIndex;

            int limit = this.firstDisplayedIndex + this.maxDisplayableItems;
            limit = (limit > ListItems.Count) ? ListItems.Count : limit;

            for(int n = this.firstDisplayedIndex; n < limit; n++)
            {
                Texture2D texture;
                Color textColor;

                if (n == selectedIndex)
                {
                    texture = this.rowSelectedBackgroundTexture;
                    textColor = this.rowSelectedTextColor;
                }
                else if ((n + 1) % 2 == 0)
                {
                    texture = this.rowAltBackgroundTexture;
                    textColor = this.rowAltTextColor;
                }
                else
                {
                    texture = this.rowBackgroundTexture;
                    textColor = this.rowTextColor;
                }

                int dw = texture.Width;
                int dh = texture.Height;

                int offset = (dh * (n - this.firstDisplayedIndex)) + ((n - this.firstDisplayedIndex) + 1);

                spriteBatch.Draw(texture, new Rectangle(dx, dy + offset, dw, dh), Color.White);
                spriteBatch.DrawString(base.Font, ListItems[n], new Vector2(dx + 4, dy + offset + 2), textColor);
            }

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = oldScissorRectangle;
            spriteBatch.Begin();

            if (this.firstDisplayedIndex > 0)
            {
                int ubx = base.DrawRectangle.X + this.DrawRectangle.Width - this.upButtonTexture.Width;
                int uby = base.DrawRectangle.Y;
                int ubw = this.upButtonTexture.Width;
                int ubh = this.upButtonTexture.Height;

                spriteBatch.Draw(this.upButtonTexture, new Rectangle(ubx, uby, ubw, ubh), Color.White);
            }

            if (this.firstDisplayedIndex + this.maxDisplayableItems <= ListItems.Count)
            {
                int dbx = base.DrawRectangle.X + this.DrawRectangle.Width - this.downButtonTexture.Width;
                int dby = base.DrawRectangle.Y + this.DrawRectangle.Height - this.downButtonTexture.Height;
                int dbw = this.downButtonTexture.Width;
                int dbh = this.downButtonTexture.Height;

                spriteBatch.Draw(this.downButtonTexture, new Rectangle(dbx, dby, dbw, dbh), Color.White);
            }
        }

        public virtual int AddItem(string item)
        {
            ListItems.Add(item);

            return ListItems.Count - 1;
        }

        public virtual void SelectedItem(int index)
        {
            if (index >= 0 && index < ListItems.Count)
            {
                this.selectedIndex = index;
            }
            else
            {
                this.selectedIndex = 0;
            }
        }

        public virtual void SelectedAndDisplayItem(int index)
        {
            if (index >= 0 && index < ListItems.Count)
            {
                SelectedItem(index);
                this.firstDisplayedIndex = index;
            }
            else
            {
                SelectedItem(0);
                this.firstDisplayedIndex = 0;
            }
        }

        public virtual void DisplayItem(int index)
        {
            if (index >= 0 && index < ListItems.Count)
            {
                this.firstDisplayedIndex = index;
            }
            else
            {
                this.firstDisplayedIndex = 0;
            }
        }

        #region IPanel Members


        public void LoadDesignElements()
        {
            base.BackdropTexture = base.GetBackdropTexture("List-Backdrop");
            base.Font = base.GetFont("List-Font");

            this.rowTextColor = base.GetColor("List-RowTextColor");
            this.rowAltTextColor = base.GetColor("List-RowAltTextColor");
            this.rowSelectedTextColor = base.GetColor("List-RowSelectedTextColor");

            this.upButtonTexture = base.GetTexture("List-UpButton");
            this.downButtonTexture = base.GetTexture("List-DownButton");
        }

        public void Load(XmlNode xmlNode)
        {
            LoadFromXML(xmlNode);
            upButtonDepressed = false;
            downButtonDepressed = false;

            BuildRowBackgroundTextures();

            maxDisplayableItems = (int)Math.Ceiling((double)this.itemDisplayRectangle.Height / (double)this.rowBackgroundTexture.Height);
        }

        public void LoadEventHandlers(XmlNode xmlNode)
        {
            
        }

        public void LoadDrawRectangle(XmlNode xmlNode)
        {
            this.itemDisplayRectangle = new Rectangle();

            itemDisplayRectangle.X = base.DrawRectangle.X + 3;
            itemDisplayRectangle.Y = base.DrawRectangle.Y + 2;
            itemDisplayRectangle.Width = base.DrawRectangle.Width - this.upButtonTexture.Width - 5;
            itemDisplayRectangle.Height = base.DrawRectangle.Height - 6;
        }

        #endregion
    }
}
