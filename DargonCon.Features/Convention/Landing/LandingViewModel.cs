using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.UserDisplay;

namespace DragonCon.Features.Convention.Landing
{
    public class LandingViewModel
    {
        public IList<DynamicSlideItem>  Slides { get; set; } = new List<DynamicSlideItem>();
    }
}
