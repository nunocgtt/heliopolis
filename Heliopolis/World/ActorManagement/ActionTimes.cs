using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentClasses;
using Microsoft.Xna.Framework.Content;

namespace Heliopolis.World
{
    public class ActionTimes : IFactory
    {
        private static Dictionary<string, TimeSpan> _actionTiming;

        public void LoadTemplatesFromContent(ContentManager contentManager, GameWorld owner, string contentFile)
        {
            _actionTiming = contentManager.Load<List<ActionTime>>(contentFile).ToDictionary(p => p.Name, q => new TimeSpan(0,0,0,0,q.Milliseconds));
        }

        private ActionTimes()
        {
        }

        public static ActionTimes Instance;

        static ActionTimes()
        {
            Instance = new ActionTimes();
        }

        public TimeSpan GetActionTime(string action)
        {
            return _actionTiming[action];
        }
    }
}
