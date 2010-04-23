using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heliopolis.Utilities;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{
    [Serializable]
    public class BuildingJobParameters : JobParameters
    {
        private Building targetBuilding;

        public BuildingJobParameters(Building _targetBuilding)
            : base()
        {
            targetBuilding = _targetBuilding;
        }

        public Building TargetBuilding
        {
            get { return targetBuilding; }
        }

        public override MovementDestination<Point> GetJobAcccessPosition(int areaId)
        {
            return new MovementDestination<Point>(targetBuilding.Position, targetBuilding.ConstructionPoints(areaId));
        }

        public override bool RequiresPositionalAccess()
        {
            return true;
        }
    }
}
