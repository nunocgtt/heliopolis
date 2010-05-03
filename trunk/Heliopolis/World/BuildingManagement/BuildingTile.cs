using System;
using Microsoft.Xna.Framework;

namespace Heliopolis.World.BuildingManagement
{
    /// <summary>
    /// Enumeration of the various states of a building tile.
    /// </summary>
    public enum BuildingTileType
    {
        /// <summary>
        /// Tile is able to store items.
        /// </summary>
        Storage,
        /// <summary>
        /// Tile is able to facilitate item construction.
        /// </summary>
        Construction,
        /// <summary>
        /// Tile does nothing.
        /// </summary>
        Nothing
    }

    /// <summary>
    /// Represents a single tile of a building.
    /// </summary>
    [Serializable]
    public class BuildingTile
    {
        /// <summary>
        /// The type of the tile.
        /// </summary>
        public BuildingTileType BuildingTileType;
        /// <summary>
        /// If this tile is physically accessable.
        /// </summary>
        public bool CanAccess;
        /// <summary>
        /// The texture of this tile.
        /// </summary>
        public string Texture;
        /// <summary>
        /// The position of this tile relative to the top left of the building.
        /// </summary>
        public Point Position;
        /// <summary>
        /// The amount of item space.
        /// </summary>
        public int ItemSpace = 0;
    }
}
