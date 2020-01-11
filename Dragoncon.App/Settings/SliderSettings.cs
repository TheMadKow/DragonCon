using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Modeling.Models.UserDisplay;

namespace DragonCon.App.Settings
{
    public class SliderSettings
    {
        public string Title { get; set; } = string.Empty;
        public IList<DynamicSlideItem> Slides { get; set; } = new List<DynamicSlideItem>();
    }
}
