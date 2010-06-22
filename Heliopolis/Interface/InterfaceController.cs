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
        private Keys[] oldPressedKeys;

        public InterfaceController(InterfaceModel model, Game gameOwner, IsometricEngine engine)
        {
            _interfaceModel = model;
            _game = gameOwner;
            _engine = engine;
            oldPressedKeys = Keyboard.GetState().GetPressedKeys();
        }

        public void Update(GameTime gameTime)
        {
            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
            
            foreach (var oldPressedKey in oldPressedKeys)
            {
                if (!pressedKeys.Contains(oldPressedKey))
                {
                    KeyUp(oldPressedKey);
                }
            }

            foreach (var pressedKey in pressedKeys)
            {
                if (!oldPressedKeys.Contains(pressedKey))
                {
                    KeyDown(pressedKey);
                }
            }

            oldPressedKeys = pressedKeys;

            _interfaceModel.SetNewMousePosition(new Point(Mouse.GetState().X, Mouse.GetState().Y), _engine);
            
            int moveSpeed = MoveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                moveSpeed *= 2;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                _interfaceModel.CameraPos.Y += moveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                _interfaceModel.CameraPos.Y -= moveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                _interfaceModel.CameraPos.X += moveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                _interfaceModel.CameraPos.X -= moveSpeed;
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
            if (Keyboard.GetState().IsKeyDown(Keys.H))
                _interfaceModel.SelectTreesForHarvest();

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
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (_interfaceModel.MouseDown)
                    _interfaceModel.EndSelection(); 
            }
            _interfaceModel.UpdateInternalMetrics(gameTime);
        }

        private void KeyDown(Keys key)
        {
            if (key == Keys.Space)
            {
                _interfaceModel.StartMoveCamera();
            }
        }

        private void KeyUp(Keys key)
        {
            if (key == Keys.P)
            {
                _interfaceModel.TogglePause();
            }
            if (key == Keys.Space)
            {
                _interfaceModel.FinishMoveCamera();
            }

        }
    }
}
