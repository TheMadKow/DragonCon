using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.UserDisplay;

namespace DragonCon.Features.Management.Displays
{
    public class DisplaysViewModel
    {
        public List<DynamicSponsorItem> Sponsors { get; set; } = new List<DynamicSponsorItem>();
        public List<DynamicSlideItem> Slides { get; set; } = new List<DynamicSlideItem>();
        public List<DynamicUpdateItem> Updates { get; set; } = new List<DynamicUpdateItem>();

        public DynamicEnglish English = new DynamicEnglish();
        public DynamicDays Days = new DynamicDays();
        public DynamicLocation Location = new DynamicLocation();
    }
}
