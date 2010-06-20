using System;
using System.Collections.Generic;

namespace ContentClasses
{
    [Serializable]
    public class ActionTime
    {
        public string Name { get; set; }
        public int Milliseconds { get; set; }
    }
}