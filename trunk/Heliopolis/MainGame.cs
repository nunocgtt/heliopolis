using Heliopolis.GraphicsEngine;
using Heliopolis.Interface;
using Heliopolis.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heliopolis.GraphicsEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        private readonly Point _screenSize;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly IsometricEngine _isometricEngine = new IsometricEngine();

        private InterfaceModel _interfaceModel;
        private InterfaceController _interfaceController;
        private InterfaceView _interfaceView;

        public MainGame()
        {
            _screenSize = new Point(1280, 720);
            _graphics = new GraphicsDeviceManager(this)
                            {
                                PreferredBackBufferWidth = _screenSize.X,
                                PreferredBackBufferHeight = _screenSize.Y
                            };
            Content.RootDirectory = "Content";  
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
            _isometricEngine.Initialize(GameWorld.Instance.Environment);
            _isometricEngine.AddTileProvider(_interfaceView);
            _isometricEngine.AddTileProvider(GameWorld.Instance.Environment);
            IsMouseVisible = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _interfaceModel = new InterfaceModel(_screenSize, GameWorld.Instance, this);
            _interfaceController = new InterfaceController(_interfaceModel, this, _isometricEngine);
            _interfaceView = new InterfaceView(_interfaceModel, _isometricEngine);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _isometricEngine.LoadContent(Content);
            _interfaceView.LoadContent(Content);
            GameWorld.Instance.LoadGameDescription(Content);
            GameWorld.Instance.LoadTestWorld();
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
            GameWorld.Instance.Tick(gameTime);
            _interfaceController.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _interfaceView.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
