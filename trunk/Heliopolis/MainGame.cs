using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Heliopolis.Interface;
using Heliopolis.Engine;
using Heliopolis.World;

namespace Heliopolis
{
    public enum SelectionState
    {
        Single,
        PlaceBuilding,
        Line,
        Area,
        None
    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        private const int TargetFrameRate = 60;
        private Point screenSize;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private IsometricEngine isometricEngine = new IsometricEngine();

        private InterfaceModel interfaceModel;
        private InterfaceController interfaceController;
        private InterfaceView interfaceView;

        public MainGame()
        {
            screenSize = new Point(1280, 720);
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = screenSize.X;
            graphics.PreferredBackBufferHeight = screenSize.Y;
            Content.RootDirectory = "Content";
            interfaceModel = new InterfaceModel(screenSize);
            interfaceController = new InterfaceController(interfaceModel, this, isometricEngine);
            interfaceView = new InterfaceView(interfaceModel, isometricEngine);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            isometricEngine.Initialize(GameWorld.Instance.Environment);
            isometricEngine.AddTileProvider(interfaceView);
            isometricEngine.AddTileProvider(GameWorld.Instance.Environment);
            this.IsMouseVisible = true;
            GameWorld.Instance.LoadTestWorld();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            isometricEngine.LoadContent(Content);
            interfaceView.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            interfaceController.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            interfaceView.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
