using System.Collections.Generic;
using Heliopolis.GraphicsEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Heliopolis.Interface
{
    public class InterfaceView : IIsometricTileProvider
    {
        private SpriteFont _hudFont;

        private readonly InterfaceModel _interfaceModel;
        private readonly IsometricEngine _engine;

        public InterfaceView(InterfaceModel model, IsometricEngine gameEngine)
        {
            _interfaceModel = model;
            _engine = gameEngine;
        }

        public void LoadContent(ContentManager contentManager)
        {
            _hudFont = contentManager.Load<SpriteFont>("Fonts/Hud");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _engine.DrawWorld(spriteBatch, _interfaceModel.CameraPos, _interfaceModel.ZoomLevel, _interfaceModel.ScreenSize);
            spriteBatch.DrawString(_hudFont, 
                string.Format("X: {0} Y: {1} FPS:{2} {3}", 
                    _interfaceModel.MouseXyPoint.X, 
                    _interfaceModel.MouseXyPoint.Y,
                    _interfaceModel.Fps,
                    _interfaceModel.GameIsPaused ? "Paused" : "" ), 
                new Vector2(0, 0), Color.White);
        }

        #region IIsometricTileProvider Members

        public List<string> GetTexturesToDraw(Point position)
        {
            List<string> returnMe = new List<string>();
            if (_interfaceModel.SelectionTiles.Contains(position))
            {
                returnMe.Add("selection1");
            }
            return returnMe;
        }

        #endregion
    }
}
