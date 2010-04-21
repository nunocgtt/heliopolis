﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Heliopolis.Engine
{
    public class Iso2D
    {
        /// <summary>
        /// Converts a point in 'screen space' to a point in 'tile space'.
        /// </summary>
        /// <param name="screenPoint">The point in 'screen space' you wish to convert.</param>
        /// <param name="tileScreenWidth">The width of a tile in 'screen space'. How wide the tile is on the screen from its left-most point to its right-most point.</param>
        /// <param name="tileScreenHeight">The height of a tile in 'screen space'. How heigh the tile is on the screen from its upper-most point to its lower-most point.</param>
        /// <returns>A point in tile space. The row and column of the tile in the tile map.</returns>
        public static Point ConvertScreenToTile(Point screenPoint, int tileScreenWidth, int tileScreenHeight)
        {
            return ConvertScreenToTile(screenPoint, tileScreenWidth, tileScreenHeight, Point.Zero, Point.Zero);
        }

        /// <summary>
        /// Converts a point in 'screen space' to a point in 'tile space'.
        /// </summary>
        /// <param name="screenPoint">The point in 'screen space' you wish to convert.</param>
        /// <param name="tileScreenWidth">The width of a tile in 'screen space'. How wide the tile is on the screen from its left-most point to its right-most point.</param>
        /// <param name="tileScreenHeight">The height of a tile in 'screen space'. How heigh the tile is on the screen from its upper-most point to its lower-most point.</param>
        /// <param name="screenOriginPoint">The point in 'screen space' that the first tile originates. This is the point that the top-left most point of the tile will be place in 'screen space', the top-left of the sprite, not the top-left point of the tile it self.</param>
        /// <returns>A point in tile space. The row and column of the tile in the tile map.</returns>
        public static Point ConvertScreenToTile(Point screenPoint, int tileScreenWidth, int tileScreenHeight, Point screenOriginPoint)
        {
            return ConvertScreenToTile(screenPoint, tileScreenWidth, tileScreenHeight, screenOriginPoint, Point.Zero);
        }

        /// <summary>
        /// Converts a point in 'screen space' to a point in 'tile space'.
        /// </summary>
        /// <param name="screenPoint">The point in 'screen space' you wish to convert.</param>
        /// <param name="tileScreenWidth">The width of a tile in 'screen space'. How wide the tile is on the screen from its left-most point to its right-most point.</param>
        /// <param name="tileScreenHeight">The height of a tile in 'screen space'. How heigh the tile is on the screen from its upper-most point to its lower-most point.</param>
        /// <param name="screenOriginPoint">The point in 'screen space' that the first tile originates. This is the point that the top-left most point of the tile will be place in 'screen space', the top-left of the sprite, not the top-left point of the tile it self.</param>
        /// <param name="screenOffset">An offset to be applied to the origin point. Typically used for scrolling.</param>
        /// <returns>A point in tile space. The row and column of the tile in the tile map.</returns>
        public static Point ConvertScreenToTile(Point screenPoint, int tileScreenWidth, int tileScreenHeight, Point screenOriginPoint, Point screenOffset)
        {
            double tw, th, tx, ty;
            double sx, sy;

            tw = tileScreenWidth;   // HOW MANY COLUMNS TILE OCCUPIES IN SCREEN SPACE
            th = tileScreenHeight;  // HOW MANY ROWS TILE OCCUPIES IN SCREEN SPACE

            sx = screenPoint.X - (screenOriginPoint.X + screenOffset.X);    // COLUMN IN SCREEN SPACE
            sy = screenPoint.Y - (screenOriginPoint.Y + screenOffset.Y);    // ROW IN SCREEN SPACE

            tx = Math.Round((sx / tw) + (sy / th)) - 1;     // COLUMN MOUSE OCCUPIES IN TILE SPACE
            ty = Math.Round((-sx / tw) + (sy / th));        // ROW MOUSE OCCUPIES IN TILE SPACE

            //sx = Math.Truncate((tw / 2) * tx - (tw / 2) * ty) + 400;    // FIRST COLUMN TILE OCCUPIES IN SCREEN SPACE (X)
            //sy = Math.Truncate((th / 2) * tx + (th / 2) * ty);  // FIRST ROW TILE OCCUPIES IN SCREEN SPACE (Y)

            return new Point((int)tx, (int)ty);
        }

        /// <summary>
        /// Converts a point in 'tile space' to a point in 'screen space'.
        /// </summary>
        /// <param name="screenPoint">The point in 'tile space' you wish to convert.</param>
        /// <param name="tileScreenWidth">The width of a tile in 'screen space'. How wide the tile is on the screen from its left-most point to its right-most point.</param>
        /// <param name="tileScreenHeight">The height of a tile in 'screen space'. How heigh the tile is on the screen from its upper-most point to its lower-most point.</param>
        /// <returns>A point in 'screen space'. The x-y coordinates of the top-left of the tile on the screen.</returns>
        public static Point ConvertTileToScreen(Point tilePoint, int tileScreenWidth, int tileScreenHeight)
        {
            return ConvertTileToScreen(tilePoint, tileScreenWidth, tileScreenHeight, Point.Zero, Point.Zero);
        }

        /// <summary>
        /// Converts a point in 'tile space' to a point in 'screen space'.
        /// </summary>
        /// <param name="screenPoint">The point in 'tile space' you wish to convert.</param>
        /// <param name="tileScreenWidth">The width of a tile in 'screen space'. How wide the tile is on the screen from its left-most point to its right-most point.</param>
        /// <param name="tileScreenHeight">The height of a tile in 'screen space'. How heigh the tile is on the screen from its upper-most point to its lower-most point.</param>
        /// <param name="screenOriginPoint">The point in 'screen space' that the first tile originates. This is the point that the top-left most point of the tile will be place in 'screen space', the top-left of the sprite, not the top-left point of the tile it self.</param>
        /// <returns>A point in 'screen space'. The x-y coordinates of the top-left of the tile on the screen.</returns>
        public static Point ConvertTileToScreen(Point tilePoint, int tileScreenWidth, int tileScreenHeight, Point screenOriginPoint)
        {
            return ConvertTileToScreen(tilePoint, tileScreenWidth, tileScreenHeight, screenOriginPoint, Point.Zero);
        }

        /// <summary>
        /// Converts a point in 'tile space' to a point in 'screen space'.
        /// </summary>
        /// <param name="screenPoint">The point in 'tile space' you wish to convert.</param>
        /// <param name="tileScreenWidth">The width of a tile in 'screen space'. How wide the tile is on the screen from its left-most point to its right-most point.</param>
        /// <param name="tileScreenHeight">The height of a tile in 'screen space'. How heigh the tile is on the screen from its upper-most point to its lower-most point.</param>
        /// <param name="screenOriginPoint">The point in 'screen space' that the first tile originates. This is the point that the top-left most point of the tile will be place in 'screen space', the top-left of the sprite, not the top-left point of the tile it self.</param>
        /// <param name="screenOffset">An offset to be applied to the origin point. Typically used for scrolling.</param>
        /// <returns>A point in 'screen space'. The x-y coordinates of the top-left of the tile on the screen.</returns>
        public static Point ConvertTileToScreen(Point tilePoint, int tileScreenWidth, int tileScreenHeight, Point screenOriginPoint, Point screenOffset)
        {
            double tw, th, tx, ty;
            double sx, sy;

            tw = tileScreenWidth;   // HOW MANY COLUMNS TILE OCCUPIES IN SCREEN SPACE
            th = tileScreenHeight;  // HOW MANY ROWS TILE OCCUPIES IN SCREEN SPACE

            tx = tilePoint.X;
            ty = tilePoint.Y;

            sx = Math.Truncate((tw / 2) * tx - (tw / 2) * ty) + (screenOriginPoint.X + screenOffset.X);     // FIRST COLUMN TILE OCCUPIES IN SCREEN SPACE (X)
            sy = Math.Truncate((th / 2) * tx + (th / 2) * ty) + (screenOriginPoint.Y + screenOffset.Y);     // FIRST ROW TILE OCCUPIES IN SCREEN SPACE (Y)

            return new Point((int)sx, (int)sy);
        }
    }

}
