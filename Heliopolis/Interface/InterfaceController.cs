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
        private const int MoveSpeed = 8;
        private readonly InterfaceModel _interfaceModel;
        private readonly Game _game;
        private readonly IsometricEngine _engine;
        private Keys[] oldPressedKeys;
        private bool _leftMouseDown = false;
        private bool _rightMouseDown = false;

        public InterfaceController(InterfaceModel model, Game gameOwner, IsometricEngine engine)
        {
            _interfaceModel = model;
            _game = gameOwner;
            _engine = engine;
            oldPressedKeys = Keyboard.GetState().GetPressedKeys();
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                _game.Exit();

            bool uiCaughtEvent = _interfaceModel.CatchUIEvents();

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
            KeyCurrentlyPressed(pressedKeys);
            oldPressedKeys = pressedKeys;

            _interfaceModel.SetNewMousePosition(new Point(Mouse.GetState().X, Mouse.GetState().Y), _engine);

            if (!uiCaughtEvent)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (!_leftMouseDown)
                        LeftMouseButtonDown();
                    LeftMouseButtonPressed();
                    _leftMouseDown = true;
                }
                else if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    if (_leftMouseDown)
                        LeftMouseButtonUp();
                    _leftMouseDown = false;
                }
                if (Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    if (!_rightMouseDown)
                        RightMouseButtonDown();
                    RightMouseButtonPressed();
                    _rightMouseDown = true;
                }
                else if (Mouse.GetState().RightButton == ButtonState.Released)
                {
                    if (_rightMouseDown)
                        RightMouseButtonUp();
                    _rightMouseDown = false;
                }
            }
            _interfaceModel.UpdateInternalMetrics(gameTime);
        }

        private void KeyCurrentlyPressed(IEnumerable<Keys> pressedKeys)
        {
            int cameraMoveSpeed = MoveSpeed;

            foreach (var pressedKey in pressedKeys)
            {
                switch (pressedKey)
                {
                    case Keys.RightShift :
                        cameraMoveSpeed *= 2;
                        break;
                    case Keys.Down:
                        _interfaceModel.CameraPos.Y += cameraMoveSpeed;
                        break;
                    case Keys.Up:
                        _interfaceModel.CameraPos.Y -= cameraMoveSpeed;
                        break;
                    case Keys.Right:
                        _interfaceModel.CameraPos.X += cameraMoveSpeed;
                        break;
                    case Keys.Left:
                        _interfaceModel.CameraPos.X -= cameraMoveSpeed;
                        break;
                }
            }
        }

        private void KeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.Space:
                    _interfaceModel.StartMoveCamera();
                    break;
            }
        }

        private void KeyUp(Keys key)
        {
            switch (key)
            {
                case Keys.P:
                    _interfaceModel.TogglePause();
                    break;
                case Keys.Space:
                    _interfaceModel.FinishMoveCamera();
                    break;
                case Keys.Add:
                    _interfaceModel.ZoomedIn = true;
                    break;
                case Keys.Subtract:
                    _interfaceModel.ZoomedIn = false;
                    break;
            }
        }

        public void LeftMouseButtonDown()
        {
            _interfaceModel.StartSelection();
        }

        public void LeftMouseButtonUp()
        {
            _interfaceModel.EndSelection(); 
        }

        public void LeftMouseButtonPressed()
        {
            
        }

        public void RightMouseButtonDown()
        {

        }

        public void RightMouseButtonUp()
        {

        }

        public void RightMouseButtonPressed()
        {

        }
    }
}
