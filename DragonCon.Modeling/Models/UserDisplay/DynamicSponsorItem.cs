using System;
using System.Collections.Generic;
using System.Text;

namespace DragonCon.Modeling.Models.UserDisplay
{
    public class DynamicSponsorItem : DynamicDisplayItem
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string SponsorUrl { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
    }
}
