using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using System;
using Heliopolis.Utilities;

namespace Heliopolis.World
{
    /// <summary>
    /// The varios states an Item can exist in.
    /// </summary>
    public enum ItemStates
    {
        /// <summary>
        /// The item is on the ground.
        /// </summary>
        OnGround,
        /// <summary>
        /// The item is being carried by an actor.
        /// </summary>
        BeingCarried,
        /// <summary>
        /// The item is in a building.
        /// </summary>
        InStorage,
        /// <summary>
        /// The item in an actor's storage/backpack.
        /// </summary>
        InBackpack
    }

    /// <summary>
    /// Represents an item in the game world.
    /// </summary>
    /// <remarks>Items can exist on the ground, in a building or on an actor as inventory.</remarks>
    [Serializable]
    public class Item : GameWorldObject, System.ICloneable, ISpatialIndexMember
    {
        private float weight;
        private string classification;
        private string texture;
        private Point position;
        private bool isReserved;
        private ItemStates itemState;
        private ICanHoldItem holder;
        private string itemType;

        /// <summary>
        /// The texture for rendering.
        /// </summary>
        public string Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        /// <summary>
        /// The position of this item.
        /// </summary>
        public Point Position
        {
            get { return position; }
            set
            {
                // Might need to change sections now
                Owner.SpatialTreeIndex.CheckChangeSection(position,value,this, SpatialObjectType.Item, this.itemType);
                position = value;
            }
        }

        /// <summary>
        /// If this item has been reserved by another object.
        /// </summary>
        public bool IsReserved
        {
            get { return isReserved; }
            set { isReserved = value; }
        }

        /// <summary>
        /// The current item state.
        /// </summary>
        public ItemStates ItemState
        {
            get { return itemState; }
            set {
                // Item is being picked up
                if ((value == ItemStates.BeingCarried || value == ItemStates.InBackpack) && (itemState == ItemStates.OnGround || itemState == ItemStates.InStorage))
                {
                    Owner.SpatialTreeIndex.RemoveFromSection(this.position, this, SpatialObjectType.Item, itemType);
                }
                // Item is being put down
                if ((value == ItemStates.OnGround || value == ItemStates.InStorage) && (itemState == ItemStates.BeingCarried || itemState == ItemStates.InBackpack))
                {
                    Owner.SpatialTreeIndex.AddToSection(this.position, this, SpatialObjectType.Item, itemType);
                }
                itemState = value; 
            }
        }

        /// <summary>
        /// If this item is being held, this is the ICanHoldItem who is holding it.
        /// </summary>
        public ICanHoldItem Holder
        {
            get { return holder; }
            set { holder = value; }
        }

        /// <summary>
        /// The class of item.
        /// </summary>
        public string Classification
        {
            get { return classification; }
        }

        /// <summary>
        /// The type of item.
        /// </summary>
        public string ItemType
        {
            get { return itemType; }
        }

        /// <summary>
        /// Initialises a new instance of the Item class.
        /// </summary>
        /// <param name="_weight">The weight.</param>
        /// <param name="_class">The class.</param>
        /// <param name="_texture">The texture.</param>
        /// <param name="_itemType">The type.</param>
        /// <param name="_owner">The owning game world.</param>
        public Item(float _weight, string _class, string _texture, string _itemType, GameWorld _owner) : base(_owner)
        {
            itemType = _itemType;
            weight = _weight;
            classification = _class;
            texture = _texture;
            isReserved = false;
            itemState = ItemStates.OnGround;
            holder = null;
            position = new Point(-1,-1);
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
