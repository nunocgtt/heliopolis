using System;
using System.Collections.Generic;

namespace ContentClasses
{
    [Serializable]
    public class ActorTemplate
    {
        public string Name { get; set; }
        public string Texture { get; set; }
        public int Hitpoints { get; set; }
        public List<string> Properties { get; set; }
        public List<string> Jobs { get; set; }
    }
}