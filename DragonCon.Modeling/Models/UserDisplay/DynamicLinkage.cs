using System;
using System.Collections.Generic;
using System.Text;

namespace DragonCon.Modeling.Models.UserDisplay
{
    public class DynamicLinkage : DynamicDisplayItem
    {
        public string SuggestGuidelinesLink { get; set; } = string.Empty;
        public string VolunteerLink { get; set; } = string.Empty;
        public string Yad2FormLink { get; set; } = string.Empty;
        public string ProgramLink { get; set; } = string.Empty;
        public string ProgramImage { get; set; } = string.Empty;
    }
}
