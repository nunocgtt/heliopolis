using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Xml;
using System;
using Heliopolis.Utilities;

namespace Heliopolis.World
{
    ///// <summary>
    ///// Vector for the various directions one can move in the game world.
    ///// </summary>
    //public enum tileDirection
    //{
    //    /// <summary>
    //    /// Left.
    //    /// </summary>
    //    Left,
    //    /// <summary>
    //    /// Right.
    //    /// </summary>
    //    Right,
    //    /// <summary>
    //    /// Up.
    //    /// </summary>
    //    Top,
    //    /// <summary>
    //    /// Down.
    //    /// </summary>
    //    Bottom
    //}

    /// <summary>
    /// Represents a single tile in the game environment.
    /// </summary>
    [Serializable]
    public class EnvironmentTile : GameWorldObject, System.ICloneable, ISpatialIndexMember
    {
        private int areaID;
        private string texture;
        private Point position;
        private string resource;
        private float resourceLeft;
        private string exhaustedTile;
        private bool canAccess;
        private List<EnvironmentTile> adjacentTiles;

        /// <summary>
        /// The area ID of this tile.
        /// </summary>
        public int AreaID
        {
            get { return areaID; }
            set { areaID = value; }
        }

        /// <summary>
        /// A list of all adjacent tiles.
        /// </summary>
        public List<EnvironmentTile> AdjacentTiles
        {
            get { return adjacentTiles; }
            set { adjacentTiles = value; }
        }
 
        /// <summary>
        /// If, from this tile, a particular direction can be accessed.
        /// </summary>
        /// <param name="direction">The direction to check.</param>
        /// <returns>Returns true if access is available.</returns>
        public bool TileCanAccess(Direction direction)
        {
            if (adjacentTiles[(int)direction] == null)
                return false;
            else
                return adjacentTiles[(int)direction].CanAccess;
        }

        /// <summary>
        /// The tile to the left.
        /// </summary>
        public EnvironmentTile LeftTile
        {
            get { return adjacentTiles[(int)Direction.West]; }
            set { adjacentTiles[(int)Direction.West] = value; }
        }
        /// <summary>
        /// The tile to the right.
        /// </summary>
        public EnvironmentTile RightTile
        {
            get { return adjacentTiles[(int)Direction.East]; }
            set { adjacentTiles[(int)Direction.East] = value; }
        }
        /// <summary>
        /// The tile above.
        /// </summary>
        public EnvironmentTile TopTile
        {
            get { return adjacentTiles[(int)Direction.North]; }
            set { adjacentTiles[(int)Direction.North] = value; }
        }
        /// <summary>
        /// The tile below.
        /// </summary>
        public EnvironmentTile BottomTile
        {
            get { return adjacentTiles[(int)Direction.South]; }
            set { adjacentTiles[(int)Direction.South] = value; }
        }
        /// <summary>
        /// The texture of this tile.
        /// </summary>
        public string Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        /// <summary>
        /// Any resources on this tile that can be harvested.
        /// </summary>
        public string Resource
        {
            get { return resource; }
            set { resource = value; }
        }
        /// <summary>
        /// If this tile can be access by actors.
        /// </summary>
        public bool CanAccess
        {
            get { return canAccess; }
            set
            {
                if (value != canAccess)
                {
                    canAccess = value;
                    owner.Environment.ManageAccessStateChange(this);
                }
                else
                    canAccess = value;
            }
        }
        /// <summary>
        /// The amount of resources left.
        /// </summary>
        public float ResourceLeft
        {
            get { return resourceLeft; }
            set { resourceLeft = value; }
        }
        /// <summary>
        /// The game world position of this tile.
        /// </summary>
        public Point Position
        {
            get { return position; }
            set { position = value; }
        }
        /// <summary>
        /// If the resources run out, what tile this one will become.
        /// </summary>
        public string ExhaustedTile
        {
            get { return exhaustedTile; }
            set { exhaustedTile = value; }
        }

        /// <summary>
        /// Returns if this tile should get rendered.
        /// </summary>
        public bool ShouldBeRendered
        {
            get { return true; }
        }

        /// <summary>
        /// Initialises a new instance of the EnvironmentTile class.
        /// </summary>
        /// <param name="_texture">The texture.</param>
        /// <param name="_resource">Any resources to harvest.</param>
        /// <param name="_resourceLeft">The amount of resources left.</param>
        /// <param name="_exhaustedTile">Resources exhausted tile.</param>
        /// <param name="_canAccess">If actors can access this tile.</param>
        /// <param name="_owner">The game world owner.</param>
        public EnvironmentTile(string _texture, string _resource, float _resourceLeft, string _exhaustedTile, bool _canAccess, GameWorld _owner) : base(_owner)
        {
            texture = _texture;
            resource = _resource;
            resourceLeft = _resourceLeft;
            exhaustedTile = _exhaustedTile;
            canAccess = _canAccess;
            areaID = 0;
        }

        /// <summary>
        /// Returns a list of all the tiles that provide access to this tile.
        /// </summary>
        /// <returns>A list of Point.</returns>
        public List<Point> GetAccessPoints()
        {
            List<Point> returnMe = new List<Point>();
            foreach (EnvironmentTile tile in adjacentTiles)
            {
                if (tile != null)
                {
                    if (tile.CanAccess)
                    {
                        returnMe.Add(tile.Position);
                    }
                }
            }
            return returnMe;
        }

        /// <summary>
        /// Creates a copy of this tile.
        /// </summary>
        /// <returns>A Tile copy.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

}
