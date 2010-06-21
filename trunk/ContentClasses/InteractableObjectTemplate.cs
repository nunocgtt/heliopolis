using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentClasses
{
    public class InteractableObjectTemplate
    {
        public string Name { get; set; }
        public bool IsHarvestable { get; set; }
        public bool IsManufacturable { get; set; }
        public int HarvestCount { get; set; }
        public string HarvestJobType { get; set; }
        public string Texture { get; set; }
    }
}
