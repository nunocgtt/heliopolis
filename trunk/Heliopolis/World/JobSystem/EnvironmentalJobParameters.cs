using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heliopolis.Utilities;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{
    [Serializable]
    public class EnvironmentalJobParameters : JobParameters
    {
        private EnvironmentTile targetTile;

        public EnvironmentalJobParameters(EnvironmentTile _targetTile)
            : base()
        {
            targetTile = _targetTile;
        }

        public EnvironmentTile TargetTile
        {
            get { return targetTile; }
        }

        public override MovementDestination<Point> GetJobAcccessPosition(int areaId)
        {
            return new MovementDestination<Point>(targetTile.Position);
        }

        public override bool RequiresPositionalAccess()
        {
            return true;
        }
    }
}
