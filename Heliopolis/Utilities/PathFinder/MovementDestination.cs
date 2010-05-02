using System;
using System.Collections.Generic;

namespace Heliopolis.Utilities.PathFinder
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
        private T _pointToMoveTo;
        private List<T> _pointsAcceptable;
        private readonly MovementDestinationType _movementDestinationType;

        public MovementDestination(List<T> pointsAcceptable)
        {
            PointsAcceptable = pointsAcceptable;
            _movementDestinationType = MovementDestinationType.MultiPoint;
        }

        public MovementDestination(T pointToMoveTo)
        {
            PointToMoveTo = pointToMoveTo;
            _movementDestinationType = MovementDestinationType.SinglePoint;
        }

        public MovementDestination()
        {
            _movementDestinationType = MovementDestinationType.UnacessablePoint;
        }

        public T PointToMoveTo
        {
            get { return _pointToMoveTo; }
            set { _pointToMoveTo = value; }
        }

        public List<T> PointsAcceptable
        {
            get { return _pointsAcceptable; }
            set { _pointsAcceptable = value; }
        }

        public MovementDestinationType MovementDestinationType
        {
            get { return _movementDestinationType; }
        }
    }
}
