using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.UserDisplay;

namespace DragonCon.Features.Convention.Home
{
    public class HomeViewModel
    {
        public IList<DynamicSlideItem> Slides { get; set; } = new List<DynamicSlideItem>();
        public IList<DynamicUpdateItem> Updates { get; set; } = new List<DynamicUpdateItem>();
    }
}
