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
        private InterfaceModel interfaceModel;
        private Game game;
        private IsometricEngine engine;

        private int frameRate = 0;
        private int frameCounter = 0;
        private TimeSpan elapsedTime = TimeSpan.Zero;

        public InterfaceController(InterfaceModel model, Game gameOwner, IsometricEngine engine)
        {
            this.interfaceModel = model;
            this.game = gameOwner;
            this.engine = engine;
        }

        public void Update(GameTime gameTime)
        {
            interfaceModel.MousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);
            Point cameraOffset = new Point((int)(interfaceModel.CameraPos.X * -1 * interfaceModel.ZoomLevel), (int)((int)(interfaceModel.CameraPos.Y * -1 * interfaceModel.ZoomLevel)));
            interfaceModel.MouseXYPoint = Iso2D.ConvertScreenToTile(interfaceModel.MousePoint, (int)(engine.TileSize.X * interfaceModel.ZoomLevel), (int)(engine.TileSize.Y * interfaceModel.ZoomLevel), engine.FirstTileXyPosition(interfaceModel.ZoomLevel), cameraOffset);

            int moveSpeed = 5;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                interfaceModel.CameraPos.Y += moveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                interfaceModel.CameraPos.Y -= moveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                interfaceModel.CameraPos.X += moveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                interfaceModel.CameraPos.X -= moveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Add))
                interfaceModel.ZoomedIn = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
                interfaceModel.ZoomedIn = false;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                interfaceModel.CurrentSelectionState = SelectionState.Area;
            if (Keyboard.GetState().IsKeyDown(Keys.L))
                interfaceModel.CurrentSelectionState = SelectionState.Line;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                interfaceModel.CurrentSelectionState = SelectionState.Single;
            if (Keyboard.GetState().IsKeyDown(Keys.N))
                interfaceModel.CurrentSelectionState = SelectionState.None;
            if (Keyboard.GetState().IsKeyDown(Keys.B))
                interfaceModel.CurrentSelectionState = SelectionState.PlaceBuilding;

            if (interfaceModel.CameraPos.Y < 0)
                interfaceModel.CameraPos.Y = 0;
            if (interfaceModel.CameraPos.X < 0)
                interfaceModel.CameraPos.X = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!interfaceModel.MouseDown)
                    interfaceModel.StartSelection();
                else
                    interfaceModel.UpdateSelection();
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (interfaceModel.MouseDown)
                    interfaceModel.EndSelection(); 
            }

            elapsedTime += gameTime.ElapsedGameTime;
            frameCounter++;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

            interfaceModel.FPS = frameRate;
            interfaceModel.UpdateSelectionInfo();
        }

    }
}
