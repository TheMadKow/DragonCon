using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.UserDisplay;

namespace DragonCon.Features.Management.Displays
{
    public class DisplaysViewModel : DynamicDisplayItem
    {
        public List<DynamicSponsorItem> Sponsors { get; set; } = new List<DynamicSponsorItem>();
        public List<DynamicSlideItem> Slides { get; set; } = new List<DynamicSlideItem>();
        public List<DynamicUpdateItem> Updates { get; set; } = new List<DynamicUpdateItem>();

        public DynamicLinkage Linkage = new DynamicLinkage();
        public DynamicEnglish English = new DynamicEnglish();
        public DynamicProgram Program = new DynamicProgram();
        public DynamicLocation Location = new DynamicLocation();
    }
}
