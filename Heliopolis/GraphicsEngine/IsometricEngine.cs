using System;
using System.Collections.Generic;
using System.Linq;
using ContentClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Heliopolis.GraphicsEngine
{
    public class IsometricEngine
    {
        public Point TileSize { get; set; }
        public Point TileCenter { get; set; }
        private int _worldPixelWidth;
        private int _worldHeightWidth;

        private TextureManager _textureManager;

        private readonly List<IIsometricTileProvider> _tileProviders = new List<IIsometricTileProvider>();

        public void LoadContent(ContentManager contentManager)
        {
            TileSize = new Point(32, 16);
            TileCenter = new Point(16, 8);
            _textureManager = new TextureManager(contentManager);
        }

        public void Initialize(IWorld enviroment)
        {
            _worldPixelWidth = TileSize.X * (enviroment.WorldSize.X + enviroment.WorldSize.Y) / 2;
            _worldHeightWidth = enviroment.WorldSize.X;
        }

        public void AddTileProvider(IIsometricTileProvider provider)
        {
            _tileProviders.Add(provider);
        }

        public void DrawWorld(SpriteBatch spriteBatch, Point cameraPosition, float zoomLevel, Point screenSize)
        {
            int edgeCount = _worldHeightWidth + _worldHeightWidth;
            int startingLine;
            int finishingLine;
            StartAndFinishLine(out startingLine, out finishingLine, cameraPosition, zoomLevel, screenSize);
            for (int drawTileLayer = startingLine; drawTileLayer < Math.Min((edgeCount - 1), finishingLine); drawTileLayer++)
            {
                int drawlen;
                bool bottomHalfDrawing = drawTileLayer >= _worldHeightWidth;
                if (bottomHalfDrawing)
                    drawlen = _worldHeightWidth - (drawTileLayer - (_worldHeightWidth - 1));
                else
                    drawlen = drawTileLayer + 1;
                int startTile;
                int lastTile;
                GetFirstAndLastTileToDraw(drawlen, out startTile, out lastTile, cameraPosition, zoomLevel, screenSize);
                for (int j = startTile; j < lastTile; j++)
                {
                    Point tileToDraw = GetTileXyLocation(drawTileLayer, bottomHalfDrawing, j);
                    DrawLayer(drawTileLayer, drawlen, j, tileToDraw, cameraPosition, spriteBatch, zoomLevel);
                }
            }
        }

        private Point GetTileXyLocation(int tileLayer, bool bottomHalfDrawing, int tileNumberInLayer)
        {
            Point tileToDraw;
            if (bottomHalfDrawing)
                tileToDraw = new Point(tileNumberInLayer + (tileLayer - (_worldHeightWidth - 1)), (_worldHeightWidth - 1) - tileNumberInLayer);
            else
                tileToDraw = new Point(tileNumberInLayer, tileLayer - tileNumberInLayer);
            return tileToDraw;
        }

        private void StartAndFinishLine(out int startingLine, out int finishingLine, Point cameraPosition, float zoomLevel, Point screenSize)
        {
            startingLine = cameraPosition.Y / (TileSize.Y / 2) - 1;
            if (startingLine < 0)
                startingLine = 0;
            finishingLine = startingLine + ((int)(screenSize.Y / zoomLevel) / (TileSize.Y / 2)) + 2;
        }

        private void GetFirstAndLastTileToDraw(int drawlen, out int startTile, out int lastTile, Point cameraPosition, float zoomLevel, Point screenSize)
        {
            int firstXPosition = FirstXPos(drawlen) - cameraPosition.X;
            startTile = (firstXPosition * -1) / TileSize.X;
            lastTile = startTile + ((int)(screenSize.X / zoomLevel) / TileSize.X) + 1;
            lastTile = Math.Min(drawlen, lastTile);
            startTile = Math.Max(0, startTile);
        }

        private void DrawLayer(int drawTileLayer, int drawlen, int j, Point tileToDraw, Point cameraPosition, SpriteBatch spriteBatch, float zoomLevel)
        {
            int firstXPosition = FirstXPos(drawlen);
            Point drawTilePos = new Point(firstXPosition + (j * TileSize.X), drawTileLayer * TileSize.Y / 2);
            drawTilePos.X = drawTilePos.X - cameraPosition.X;
            drawTilePos.Y = drawTilePos.Y - cameraPosition.Y;

            List<IsometricTexture> texturesToDraw =
                (from provider in _tileProviders
                 from drawTextureName in provider.GetTexturesToDraw(tileToDraw)
                 select _textureManager.Textures[drawTextureName]).ToList();

            foreach (IsometricTexture drawTexture in texturesToDraw.OrderBy(p => p.ZLevel))
            {
                Point offset = new Point(TileCenter.X - drawTexture.CenterPoint.X, TileCenter.Y - drawTexture.CenterPoint.Y);
                Rectangle floorScreenRectangle = new Rectangle(drawTilePos.X + offset.X, drawTilePos.Y + offset.Y, drawTexture.Size.X, drawTexture.Size.Y);
                Rectangle finalDrawRect = Zoom(zoomLevel, floorScreenRectangle);
                Rectangle textureRect = new Rectangle();
                textureRect.X = drawTexture.TexturePointOrigin.X;
                textureRect.Y = drawTexture.TexturePointOrigin.Y;
                textureRect.Width = drawTexture.Size.X;
                textureRect.Height = drawTexture.Size.Y;
                spriteBatch.Draw(_textureManager.TextureSheets[drawTexture.TextureSheet], finalDrawRect, textureRect, Color.White);
            }
        }

        private static Rectangle Zoom(double zoomLevel, Rectangle passMe)
        {
            Rectangle returnMe =
                new Rectangle
                    {
                        Width = (int) Math.Truncate(passMe.Width*zoomLevel),
                        Height = (int) Math.Truncate(passMe.Height*zoomLevel),
                        X = (int) Math.Truncate(passMe.X*zoomLevel),
                        Y = (int) Math.Truncate(passMe.Y*zoomLevel)
                    };
            return returnMe;
        }

        private int FirstXPos(int drawlen)
        {
            return (_worldPixelWidth / 2) - (drawlen * (TileSize.X / 2));
        }

        public Point FirstTileXyPosition(float zoomLevel)
        {
            return new Point((int)(FirstXPos(1) * zoomLevel), 0);
        }
    }
}
