using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentClasses;
using Microsoft.Xna.Framework.Content;

namespace Heliopolis.World
{
    public class ActionTimes
    {
        private static Dictionary<string, TimeSpan> _actionTiming;

        public static void LoadTemplatesFromXml(ContentManager contentManager, GameWorld owner)
        {
            _actionTiming = contentManager.Load<List<ActionTime>>(@"GameWorldDefintion/actiontimes").ToDictionary(p => p.Name, q => new TimeSpan(0,0,0,0,q.Milliseconds));
        }

        public static TimeSpan GetActionTime(string action)
        {
            return _actionTiming[action];
        }
    }
}
