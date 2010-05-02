using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heliopolis.GraphicsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Heliopolis.Interface
{
    public class InterfaceController
    {
        private const int MoveSpeed = 5;
        private readonly InterfaceModel _interfaceModel;
        private readonly Game _game;
        private readonly IsometricEngine _engine;
        private int _frameRate;
        private int _frameCounter;
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        public InterfaceController(InterfaceModel model, Game gameOwner, IsometricEngine engine)
        {
            _interfaceModel = model;
            _game = gameOwner;
            _engine = engine;
        }

        public void Update(GameTime gameTime)
        {
            _interfaceModel.MousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);
            Point cameraOffset = new Point((int)(_interfaceModel.CameraPos.X * -1 * _interfaceModel.ZoomLevel), (int)((int)(_interfaceModel.CameraPos.Y * -1 * _interfaceModel.ZoomLevel)));
            _interfaceModel.MouseXyPoint = Iso2D.ConvertScreenToTile(_interfaceModel.MousePoint, (int)(_engine.TileSize.X * _interfaceModel.ZoomLevel), (int)(_engine.TileSize.Y * _interfaceModel.ZoomLevel), _engine.FirstTileXyPosition(_interfaceModel.ZoomLevel), cameraOffset);

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                _interfaceModel.CameraPos.Y += MoveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                _interfaceModel.CameraPos.Y -= MoveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                _interfaceModel.CameraPos.X += MoveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                _interfaceModel.CameraPos.X -= MoveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Add))
                _interfaceModel.ZoomedIn = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
                _interfaceModel.ZoomedIn = false;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                _interfaceModel.CurrentSelectionState = SelectionState.Area;
            if (Keyboard.GetState().IsKeyDown(Keys.L))
                _interfaceModel.CurrentSelectionState = SelectionState.Line;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                _interfaceModel.CurrentSelectionState = SelectionState.Single;
            if (Keyboard.GetState().IsKeyDown(Keys.N))
                _interfaceModel.CurrentSelectionState = SelectionState.None;
            if (Keyboard.GetState().IsKeyDown(Keys.B))
                _interfaceModel.CurrentSelectionState = SelectionState.PlaceBuilding;

            if (_interfaceModel.CameraPos.Y < 0)
                _interfaceModel.CameraPos.Y = 0;
            if (_interfaceModel.CameraPos.X < 0)
                _interfaceModel.CameraPos.X = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                _game.Exit();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!_interfaceModel.MouseDown)
                    _interfaceModel.StartSelection();
                else
                    _interfaceModel.UpdateSelection();
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (_interfaceModel.MouseDown)
                    _interfaceModel.EndSelection(); 
            }

            _elapsedTime += gameTime.ElapsedGameTime;
            _frameCounter++;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }

            _interfaceModel.Fps = _frameRate;
            _interfaceModel.UpdateSelectionInfo();
        }

    }
}
