using Heliopolis.Utilities.SpatialTreeIndexSystem;
using Microsoft.Xna.Framework;
using System;

namespace Heliopolis.World.ItemManagement
{
    /// <summary>
    /// Represents an item in the game world.
    /// </summary>
    /// <remarks>Items can exist on the ground, in a building or on an actor as inventory.</remarks>
    [Serializable]
    public class Item : GameWorldObject, ICloneable, ISpatialIndexMember
    {
        private readonly string _classification;
        private readonly string _itemType;
        private readonly float _weight; 
        private string _texture;
        private Point _position;
        private bool _isReserved;
        private ItemStates _itemState;
        private ICanHoldItem _holder;

        /// <summary>
        /// Gets the weight.
        /// </summary>
        /// <value>The weight.</value>
        public float Weight
        {
            get { return _weight; }
        }

        /// <summary>
        /// The texture for rendering.
        /// </summary>
        public string Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        /// <summary>
        /// The position of this item.
        /// </summary>
        public Point Position
        {
            get { return _position; }
            set
            {
                // Might need to change sections now
                Owner.SpatialTreeIndex.CheckChangeSection(_position,value,this, SpatialObjectType.Item, _itemType);
                _position = value;
            }
        }

        /// <summary>
        /// If this item has been reserved by another object.
        /// </summary>
        public bool IsReserved
        {
            get { return _isReserved; }
            set { _isReserved = value; }
        }

        /// <summary>
        /// The current item state.
        /// </summary>
        public ItemStates ItemState
        {
            get { return _itemState; }
            set {
                // Item is being picked up
                if ((value == ItemStates.BeingCarried || value == ItemStates.InBackpack) && (_itemState == ItemStates.OnGround || _itemState == ItemStates.InStorage))
                {
                    Owner.SpatialTreeIndex.RemoveFromSection(_position, this, SpatialObjectType.Item, _itemType);
                }
                // Item is being put down
                if ((value == ItemStates.OnGround || value == ItemStates.InStorage) && (_itemState == ItemStates.BeingCarried || _itemState == ItemStates.InBackpack))
                {
                    Owner.SpatialTreeIndex.AddToSection(_position, this, SpatialObjectType.Item, _itemType);
                }
                _itemState = value; 
            }
        }

        /// <summary>
        /// If this item is being held, this is the ICanHoldItem who is holding it.
        /// </summary>
        public ICanHoldItem Holder
        {
            get { return _holder; }
            set { _holder = value; }
        }

        /// <summary>
        /// The class of item.
        /// </summary>
        public string Classification
        {
            get { return _classification; }
        }

        /// <summary>
        /// The type of item.
        /// </summary>
        public string ItemType
        {
            get { return _itemType; }
        }

        /// <summary>
        /// Initialises a new instance of the Item class.
        /// </summary>
        /// <param name="weight">The weight.</param>
        /// <param name="classification">The class.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="itemType">The type.</param>
        /// <param name="owner">The owning game world.</param>
        public Item(float weight, string classification, string texture, string itemType, GameWorld owner) : base(owner)
        {
            _itemType = itemType;
            _classification = classification;
            _texture = texture;
            _isReserved = false;
            _itemState = ItemStates.OnGround;
            _holder = null;
            _weight = weight;
            _position = new Point(-1,-1);
        }

        /// <summary>
        /// Creates a copy of this item.
        /// </summary>
        /// <returns>A copy.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
