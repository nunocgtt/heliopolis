using Heliopolis.World.Environment;

namespace Heliopolis.World.InteractableObjects
{
    public class ManufactureInteractableObject : InteractableObject
    {
        public ManufactureInteractableObject(GameWorld owner, EnvironmentTile owningTile, string texture,
                                             string action)
            : base(owner, owningTile, texture, action)
        {
            OwningTile = owningTile;
            TimedEventDisabled = true;
            Texture = texture;
            TimedEventDisabled = true;
        }
    }
}