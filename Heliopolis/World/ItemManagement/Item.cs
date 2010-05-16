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
        private readonly string _texture;
        private Point _position;
        private bool _isReserved;
        private ItemStates _itemState;
        private ICanHoldItem _holder;

        private readonly int _storageSize;
        public int StorageSize
        {
            get { return _storageSize; }
        }

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
                if (IsMemberOfSpatialIndex)
                    Owner.SpatialTreeIndex.CheckChangeSection(value, this, new SpatialObjectKey() { ObjectType = SpatialObjectType.Item, ObjectSubtype = _itemType });
                _position = value;
            }
        }

        /// <summary>
        /// If this item has been reserved by another object.
        /// </summary>
        public bool IsReserved
        {
            get { return _isReserved; }
            set
            {
                _isReserved = value;
                UpdateSpatialIndexMembership();
            }
        }

        private bool _isMemberOfSpatialIndex;

        private bool IsMemberOfSpatialIndex
        {
            get { return _isMemberOfSpatialIndex; }
            set
            {
                if (_isMemberOfSpatialIndex && !value)
                {
                    Owner.SpatialTreeIndex.RemoveFromSection(this,  new SpatialObjectKey() { ObjectType = SpatialObjectType.Item, ObjectSubtype = _itemType});
                }
                else if (!_isMemberOfSpatialIndex && value)
                {
                    Owner.SpatialTreeIndex.AddToSection(this, new SpatialObjectKey() { ObjectType = SpatialObjectType.Item, ObjectSubtype = _itemType });
                }
                _isMemberOfSpatialIndex = value;
            }
        }

        private void UpdateSpatialIndexMembership()
        {
            IsMemberOfSpatialIndex = (_itemState == ItemStates.OnGround || _itemState == ItemStates.InStorage) &&
                                     !IsReserved;
        }

        /// <summary>
        /// The current item state.
        /// </summary>
        public ItemStates ItemState
        {
            get { return _itemState; }
            set {
                _itemState = value; 
                UpdateSpatialIndexMembership();
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
            _isMemberOfSpatialIndex = false;
            _itemState = ItemStates.Nowhere;
            _holder = null;
            _weight = weight;
            _position = new Point(-1,-1);
            _storageSize = 1;
        }

        /// <summary>
        /// Creates a copy of this item.
        /// </summary>
        /// <returns>A copy.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        #region ISpatialIndexMember Members

        public Point SpatialIndexPosition
        {
            get { return Holder.SpatialIndexPosition; }
        }

        #endregion
    }
}
