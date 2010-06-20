using System;

namespace ContentClasses
{
    [Serializable]
    public class ItemTemplate
    {
        public string Name { get; set; }
        public int Weight { get; set; }
        public string Texture { get; set; }
    }
}