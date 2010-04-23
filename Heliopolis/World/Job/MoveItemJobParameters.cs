using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heliopolis.Utilities;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{
    [Serializable]
    public class MoveItemJobParameters : JobParameters
    {
        private string targetItemType;
        private Item targetItem = null;
        private ICanHoldItem targetHolder;

        public MoveItemJobParameters(String _targetItemType, ICanHoldItem _targetHolder)
            : base()
        {
            targetItemType = _targetItemType;
            targetHolder = _targetHolder;
        }

        public Item TargetItem
        {
            get { return targetItem; }
            set { targetItem = value; }
        }

        public ICanHoldItem TargetHolder
        {
            get { return targetHolder; }
        }

        public string TargetItemType
        {
            get { return targetItemType; }
        }

        public override MovementDestination<Point> GetJobAcccessPosition(int areaId)
        {
            if (targetItem != null)
                return new MovementDestination<Point>(targetItem.Position);
            else
                throw new Exception("Can not get the position of this job because no item has been specified.");
        }

        public override bool RequiresPositionalAccess()
        {
            return false;
        }
    }
}
