using System;

namespace ContentClasses
{
    [Serializable]
    public class TileTemplate
    {
        public string Name { get; set; }
        public string Texture { get; set; }
        public bool CanAccess { get; set; }
    }
}