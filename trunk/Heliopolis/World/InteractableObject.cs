using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heliopolis.World
{
    public class InteractableObject : TimedEventor
    {
        public InteractableObject(GameWorld _owner) : base(_owner)
        {

        }

        public override void ExecuteTick(TimeSpan absoluteMilliseconds)
        {
            throw new NotImplementedException();
        }
    }
}
