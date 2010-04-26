﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Heliopolis.Engine
{
    public class IsometricEngine
    {
        public Point TileSize { get; set; }
        public Point TileCenter { get; set; }
        private int worldPixelWidth;
        private int worldHeightWidth;

        private TextureManager textureManager;

        private List<IIsometricTileProvider> tileProviders = new List<IIsometricTileProvider>();

        public void LoadContent(ContentManager contentManager)
        {
            TileSize = new Point(32, 16);
            TileCenter = new Point(16, 8);
            textureManager = new TextureManager(contentManager);
        }

        public IsometricEngine()
        {
        }

        public void Initialize(IWorld enviroment)
        {
            worldPixelWidth = TileSize.X * (enviroment.WorldSize.X + enviroment.WorldSize.Y) / 2;
            worldHeightWidth = enviroment.WorldSize.X;
        }

        public void AddTileProvider(IIsometricTileProvider provider)
        {
            tileProviders.Add(provider);
        }

        public void DrawWorld(SpriteBatch spriteBatch, Point cameraPosition, float zoomLevel, Point screenSize)
        {
            int edgeCount = worldHeightWidth + worldHeightWidth;
            int startingLine;
            int finishingLine;
            startAndFinishLine(out startingLine, out finishingLine, cameraPosition, zoomLevel, screenSize);
            for (int drawTileLayer = startingLine; drawTileLayer < Math.Min((edgeCount - 1), finishingLine); drawTileLayer++)
            {
                int drawlen = 0;
                bool bottomHalfDrawing = drawTileLayer >= worldHeightWidth;
                if (bottomHalfDrawing)
                    drawlen = worldHeightWidth - (drawTileLayer - (worldHeightWidth - 1));
                else
                    drawlen = drawTileLayer + 1;
                int startTile;
                int lastTile;
                getFirstAndLastTileToDraw(drawlen, out startTile, out lastTile, cameraPosition, zoomLevel, screenSize);
                for (int j = startTile; j < lastTile; j++)
                {
                    Point tileToDraw = getTileXYLocation(drawTileLayer, bottomHalfDrawing, j);
                    drawLayer(drawTileLayer, drawlen, j, tileToDraw, cameraPosition, spriteBatch, zoomLevel);
                }
            }
        }

        private Point getTileXYLocation(int tileLayer, bool bottomHalfDrawing, int tileNumberInLayer)
        {
            Point tileToDraw;
            if (bottomHalfDrawing)
                tileToDraw = new Point(tileNumberInLayer + (tileLayer - (worldHeightWidth - 1)), (worldHeightWidth - 1) - tileNumberInLayer);
            else
                tileToDraw = new Point(tileNumberInLayer, tileLayer - tileNumberInLayer);
            return tileToDraw;
        }

        private void startAndFinishLine(out int startingLine, out int finishingLine, Point cameraPosition, float zoomLevel, Point screenSize)
        {
            startingLine = cameraPosition.Y / (TileSize.Y / 2) - 1;
            if (startingLine < 0)
                startingLine = 0;
            finishingLine = startingLine + ((int)(screenSize.Y / zoomLevel) / (TileSize.Y / 2)) + 2;
        }

        private void getFirstAndLastTileToDraw(int drawlen, out int startTile, out int lastTile, Point cameraPosition, float zoomLevel, Point screenSize)
        {
            int firstXPosition = firstXPos(drawlen) - cameraPosition.X;
            startTile = (firstXPosition * -1) / TileSize.X;
            lastTile = startTile + ((int)(screenSize.X / zoomLevel) / TileSize.X) + 1;
            lastTile = Math.Min(drawlen, lastTile);
            startTile = Math.Max(0, startTile);
        }

        private void drawLayer(int drawTileLayer, int drawlen, int j, Point tileToDraw, Point cameraPosition, SpriteBatch spriteBatch, float zoomLevel)
        {
            int firstXPosition = firstXPos(drawlen);
            Point drawTilePos = new Point(firstXPosition + (j * TileSize.X), drawTileLayer * TileSize.Y / 2);
            drawTilePos.X = drawTilePos.X - cameraPosition.X;
            drawTilePos.Y = drawTilePos.Y - cameraPosition.Y;

            List<IsometricTexture> texturesToDraw = new List<IsometricTexture>();
            foreach (IIsometricTileProvider provider in tileProviders)
            {
                foreach (string drawTextureName in provider.GetTexturesToDraw(tileToDraw))
                    texturesToDraw.Add(textureManager.Textures[drawTextureName]);
            }

            foreach (IsometricTexture drawTexture in texturesToDraw.OrderBy(p => p.ZLevel))
            {
                Point offset = new Point(TileCenter.X - drawTexture.CenterPoint.X, TileCenter.Y - drawTexture.CenterPoint.Y);
                Rectangle floorScreenRectangle = new Rectangle(drawTilePos.X + offset.X, drawTilePos.Y + offset.Y, drawTexture.Size.X, drawTexture.Size.Y);
                Rectangle finalDrawRect = zoom(zoomLevel, floorScreenRectangle);
                Rectangle textureRect = new Rectangle();
                textureRect.X = drawTexture.TexturePointOrigin.X;
                textureRect.Y = drawTexture.TexturePointOrigin.Y;
                textureRect.Width = drawTexture.Size.X;
                textureRect.Height = drawTexture.Size.Y;
                spriteBatch.Draw(textureManager.TextureSheets[drawTexture.TextureSheet], finalDrawRect, textureRect, Color.White);
            }
        }

        private static Rectangle zoom(double zoomLevel, Rectangle passMe)
        {
            Rectangle returnMe = new Rectangle();
            returnMe.Width = (int)Math.Truncate(passMe.Width * zoomLevel);
            returnMe.Height = (int)Math.Truncate(passMe.Height * zoomLevel);
            returnMe.X = (int)Math.Truncate(passMe.X * zoomLevel);
            returnMe.Y = (int)Math.Truncate(passMe.Y * zoomLevel);
            return returnMe;
        }

        private int firstXPos(int drawlen)
        {
            return (worldPixelWidth / 2) - (drawlen * (TileSize.X / 2));
        }

        public Point FirstTileXYPosition(float zoomLevel)
        {
            return new Point((int)(firstXPos(1) * zoomLevel), 0);
        }
    }
}