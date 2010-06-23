using System.Linq;
using System.Collections.Generic;
using Heliopolis.GraphicsEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ContentClasses;

namespace Heliopolis.Interface
{
    public class InterfaceView : IIsometricTileProvider
    {
        private readonly InterfaceModel _interfaceModel;
        private readonly IsometricEngine _engine;

        private Dictionary<string, MouseCursor> _mouseCursors;
        private Dictionary<string, Texture2D> _mouseCursorTextures;

        public InterfaceView(InterfaceModel model, IsometricEngine gameEngine)
        {
            _interfaceModel = model;
            _engine = gameEngine;
            
        }

        public void LoadContent(ContentManager contentManager)
        {
            _mouseCursors =
                contentManager.Load<List<MouseCursor>>("GameWorldDefinition/mousecursors").ToDictionary(p => p.Name);
            _mouseCursorTextures = new Dictionary<string, Texture2D>();
            foreach (var mouseCursor in _mouseCursors)
            {
                _mouseCursorTextures.Add(mouseCursor.Key, contentManager.Load<Texture2D>(mouseCursor.Value.MouseTexture));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _engine.DrawWorld(spriteBatch, _interfaceModel.CameraPos, _interfaceModel.ZoomLevel, _interfaceModel.ScreenSize);
            _interfaceModel.UserInterface.Draw(spriteBatch);
            spriteBatch.Draw(_mouseCursorTextures[_interfaceModel.MouseCursor]
                ,new Vector2(_interfaceModel.MousePoint.X - _mouseCursors[_interfaceModel.MouseCursor].CenterPoint.X,
                    _interfaceModel.MousePoint.Y - _mouseCursors[_interfaceModel.MouseCursor].CenterPoint.Y)
                    ,Color.White);
        }

        #region IIsometricTileProvider Members

        public List<string> GetTexturesToDraw(Point position)
        {
            var returnMe = new List<string>();
            if (_interfaceModel.CurrentInterfaceState == InterfaceState.MakeSelection ||
                _interfaceModel.CurrentInterfaceState == InterfaceState.CurrentlyMakingSelection)
            {
                if (_interfaceModel.SelectionTiles != null)
                {
                    if (_interfaceModel.SelectionTiles.Contains(position))
                    {
                        returnMe.Add("selection1");
                    }
                }
            }
            if (_interfaceModel.CurrentInterfaceState == InterfaceState.PlacingBuilding)
            {
                if (position.X >= _interfaceModel.MousePointIsometricGrid.X
                    && position.X < _interfaceModel.MousePointIsometricGrid.X + _interfaceModel.BuildingToPlace.Size.X
                    && position.Y >= _interfaceModel.MousePointIsometricGrid.Y
                    && position.Y < _interfaceModel.MousePointIsometricGrid.Y + _interfaceModel.BuildingToPlace.Size.Y)
                {
                    Point buildingTilePosition = new Point(position.X - _interfaceModel.MousePointIsometricGrid.X, position.Y - _interfaceModel.MousePointIsometricGrid.Y);
                    returnMe.Add(_interfaceModel.BuildingToPlace.BuildingTiles[buildingTilePosition.X, buildingTilePosition.Y].Texture);
                }
            }
            return returnMe;
        }

        #endregion
    }
}
