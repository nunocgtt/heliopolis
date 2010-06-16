using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heliopolis.UILibrary
{
    public class TextLiteral : Panel, IPanel
    {
        public string Text { get; set; }
        public string Binding { get; set; }

        protected SpriteFont font;
        public override SpriteFont Font { 
            get { return this.font; }
            set { SetFont(value); }
        }

        protected string position;
        protected int offsetX;
        protected int offsetY;

        protected string lastTextMeasured;

        protected void LoadText(XmlNode xmlNode)
        {
            Text = "";

            XmlNode textNode = xmlNode.SelectSingleNode("Text");
            XmlNode binding = xmlNode.SelectSingleNode("@binding");

            if (textNode != null)
            {
                if (textNode.FirstChild != null)
                {
                    Text = textNode.FirstChild.Value;
                }
            }

            if (binding != null)
            {
                Binding = binding.Value;
            }
        }

        protected virtual void SetFont(SpriteFont font)
        {
            if (font == null)
            {
                this.font = base.GetFont("TextPanel-Font");
            }
            else
            {
                this.font = font;
            }

            Vector2 fontDimensions = Font.MeasureString("M");

            int dx = (base.DrawRectangle != null) ? base.DrawRectangle.X : 0;
            int dy = (base.DrawRectangle != null) ? base.DrawRectangle.Y : 0;

            base.DrawRectangle = new Rectangle(dx, dy, (int)fontDimensions.X, (int)fontDimensions.Y);
        }

        protected virtual void Redimension()
        {
            Vector2 fontDimensions = Font.MeasureString(Text);

            int dx = this.offsetX;
            int dy = this.offsetY;
            int dw = (int)fontDimensions.X;
            int dh = (int)fontDimensions.Y;

            DrawRectangle = PanelFactory.CalculateDrawRectangle(this.position, dx, dy, dw, dh, Parent, UserInterface);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Text != null)
            {
                if (Text != lastTextMeasured)
                {
                    Redimension();
                    lastTextMeasured = Text;
                }

                spriteBatch.DrawString(Font, Text, new Vector2(DrawRectangle.X, DrawRectangle.Y), Color);
            }
        }

        public override bool Update()
        {
            bool keepGoing = base.Update();
            if (UserInterface.GameValueProvider != null && !string.IsNullOrEmpty(Binding))
            {
                Text = UserInterface.GameValueProvider.GetGameValue(Binding);
            }
            return keepGoing;
        }

        #region IPanel Members

        public void LoadEventHandlers(XmlNode xmlNode)
        {

        }

        public void LoadDesignElements()
        {
            Color = base.GetColor("TextPanel-TextColor");

            SpriteFont font = base.GetFont("TextPanel-Font");

            SetFont(font);
        }

        public void Load(XmlNode xmlNode)
        {
            EnableClipping = false;

            LoadText(xmlNode);
        }

        public void LoadDrawRectangle(XmlNode xmlNode)
        {
            string position = "relative";

            int dx = 0;
            int dy = 0;
            int dw = 0;
            int dh = 0;

            if (xmlNode != null)
            {
                XmlNode positionAttribute = xmlNode.SelectSingleNode(@"@position");

                if (positionAttribute != null)
                {
                    position = positionAttribute.Value.ToLower();

                    XmlNode xAttribute = xmlNode.SelectSingleNode(@"@x");
                    XmlNode yAttribute = xmlNode.SelectSingleNode(@"@y");
                    XmlNode widthAttribute = xmlNode.SelectSingleNode(@"@width");
                    XmlNode heightAttribute = xmlNode.SelectSingleNode(@"@height");

                    if (xAttribute != null)
                    {
                        if (!Int32.TryParse(xAttribute.Value, out dx))
                        {
                            dx = 0;
                        }
                    }

                    if (yAttribute != null)
                    {
                        if (!Int32.TryParse(yAttribute.Value, out dy))
                        {
                            dy = 0;
                        }
                    }

                    if (widthAttribute != null)
                    {
                        if (!Int32.TryParse(widthAttribute.Value, out dw))
                        {
                            dw = 0;
                        }
                    }

                    if (heightAttribute != null)
                    {
                        if (!Int32.TryParse(heightAttribute.Value, out dh))
                        {
                            dh = 0;
                        }
                    }
                }
            }

            this.position = position;
            this.offsetX = dx;
            this.offsetY = dy;
        }

        #endregion
    }
}
