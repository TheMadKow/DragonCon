using System;
using System.Collections.Generic;
using System.Text;

namespace DragonCon.Modeling.Models.UserDisplay
{
    public class DynamicEnglish : DynamicDisplayItem
    {
        public string Location { get; set; } = string.Empty;
        public string LocationDescription{ get; set; } = string.Empty;
        public string LocationMap { get; set; } = string.Empty;

    }
}
