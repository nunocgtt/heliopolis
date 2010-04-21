using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Heliopolis.Engine;

namespace Heliopolis.Interface
{
    public class InterfaceView : IIsometricTileProvider
    {
        private SpriteFont hudFont;

        private InterfaceModel interfaceModel;
        private IsometricEngine engine;

        public InterfaceView(InterfaceModel model, IsometricEngine gameEngine)
        {
            this.interfaceModel = model;
            this.engine = gameEngine;
        }

        public void LoadContent(ContentManager contentManager)
        {
            hudFont = contentManager.Load<SpriteFont>("Fonts/Hud");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            engine.DrawWorld(spriteBatch, interfaceModel.CameraPos, interfaceModel.ZoomLevel, interfaceModel.ScreenSize);
            spriteBatch.DrawString(hudFont, 
                string.Format("X: {0} Y: {1} FPS:{2}", 
                    interfaceModel.MouseXYPoint.X, 
                    interfaceModel.MouseXYPoint.Y,
                    interfaceModel.FPS), 
                new Vector2(0, 0), Color.White);
        }

        #region IIsometricTileProvider Members

        public List<string> GetTexturesToDraw(Point position)
        {
            List<string> returnMe = new List<string>();
            if (interfaceModel.SelectionTiles.Contains(position))
            {
                returnMe.Add("selection1");
            }
            return returnMe;
        }

        #endregion
    }
}
