using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities
{
    public enum MovementDestinationType
    {
        SinglePoint,
        MultiPoint,
        UnacessablePoint
    }

    [Serializable]
    public class MovementDestination<T>
    {
        private T pointToMoveTo;
        private List<T> pointsAcceptable = null;
        private MovementDestinationType movementDestinationType;

        public MovementDestination(List<T> _pointsAcceptable)
        {
            PointsAcceptable = _pointsAcceptable;
            movementDestinationType = MovementDestinationType.MultiPoint;
        }

        public MovementDestination(T _pointToMoveTo)
        {
            PointToMoveTo = _pointToMoveTo;
            movementDestinationType = MovementDestinationType.SinglePoint;
        }

        public MovementDestination()
        {
            movementDestinationType = MovementDestinationType.UnacessablePoint;
        }

        public T PointToMoveTo
        {
            get { return pointToMoveTo; }
            set { pointToMoveTo = value; }
        }

        public List<T> PointsAcceptable
        {
            get { return pointsAcceptable; }
            set { pointsAcceptable = value; }
        }

        public MovementDestinationType MovementDestinationType
        {
            get { return movementDestinationType; }
        }
    }
}
