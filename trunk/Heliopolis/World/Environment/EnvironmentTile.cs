using System.Collections.Generic;
using System.Linq;
using Heliopolis.Utilities.PathFinder;
using Heliopolis.Utilities.SpatialTreeIndexSystem;
using Microsoft.Xna.Framework;
using System;
using Heliopolis.Utilities;

namespace Heliopolis.World.Environment
{
    /// <summary>
    /// Represents a single tile in the game environment.
    /// </summary>
    [Serializable]
    public class EnvironmentTile : GameWorldObject, System.ICloneable, ISpatialIndexMember
    {
        private int _areaId;
        private string _texture;
        private Point _position;
        private bool _canAccess;
        private List<EnvironmentTile> _adjacentTiles;
        private List<Item> _itemsOnGround = new List<Item>();
        private readonly List<IRequiresAccess> _requiringAccess = new List<IRequiresAccess>();

        public List<IRequiresAccess> RequiringAccess
        {
            get
            {
                return _requiringAccess;
            }
        }

        public List<Actor> ActorsOnTile;

        /// <summary>
        /// The area ID of this tile.
        /// </summary>
        public int AreaID
        {
            get { return _areaId; }
            set { _areaId = value; }
        }

        /// <summary>
        /// A list of all adjacent tiles.
        /// </summary>
        public List<EnvironmentTile> AdjacentTiles
        {
            get { return _adjacentTiles; }
            set { _adjacentTiles = value; }
        }
 
        /// <summary>
        /// If, from this tile, a particular direction can be accessed.
        /// </summary>
        /// <param name="direction">The direction to check.</param>
        /// <returns>Returns true if access is available.</returns>
        public bool TileCanAccess(Direction direction)
        {
            return _adjacentTiles[(int)direction] != null && _adjacentTiles[(int)direction].CanAccess;
        }

        /// <summary>
        /// The tile to the left.
        /// </summary>
        public EnvironmentTile WestTile
        {
            get { return _adjacentTiles[(int)Direction.West]; }
            set { _adjacentTiles[(int)Direction.West] = value; }
        }
        /// <summary>
        /// The tile to the right.
        /// </summary>
        public EnvironmentTile EastTile
        {
            get { return _adjacentTiles[(int)Direction.East]; }
            set { _adjacentTiles[(int)Direction.East] = value; }
        }
        /// <summary>
        /// The tile above.
        /// </summary>
        public EnvironmentTile NorthTile
        {
            get { return _adjacentTiles[(int)Direction.North]; }
            set { _adjacentTiles[(int)Direction.North] = value; }
        }
        /// <summary>
        /// The tile below.
        /// </summary>
        public EnvironmentTile SouthTile
        {
            get { return _adjacentTiles[(int)Direction.South]; }
            set { _adjacentTiles[(int)Direction.South] = value; }
        }

        /// <summary>
        /// The texture of this tile.
        /// </summary>
        public string Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        /// <summary>
        /// If this tile can be access by actors.
        /// </summary>
        public bool CanAccess
        {
            get 
            {
                return _canAccess; 
            }
            set
            {
                Owner.Environment.ManageAccessStateChange(this);
                foreach (IRequiresAccess listener in _requiringAccess)
                {
                    listener.AccessChanged(value, this.Position);
                }
                _canAccess = value;
            }
        }

        /// <summary>
        /// The game world position of this tile.
        /// </summary>
        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public InteractableObject InteractableObject { get; set; }

        /// <summary>
        /// Initialises a new instance of the EnvironmentTile class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="canAccess">If actors can access this tile.</param>
        /// <param name="owner">The game world owner.</param>
        public EnvironmentTile(string texture, bool canAccess, GameWorld owner) : base(owner)
        {
            this._texture = texture;
            this._canAccess = canAccess;
            _areaId = 0;
        }

        /// <summary>
        /// Returns a list of all the tiles that provide access to this tile.
        /// </summary>
        /// <returns>A list of Point.</returns>
        public List<Point> GetAccessPoints()
        {
            return (from tile in _adjacentTiles 
                    where tile != null 
                    where tile.CanAccess 
                    select tile.Position).ToList();
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
