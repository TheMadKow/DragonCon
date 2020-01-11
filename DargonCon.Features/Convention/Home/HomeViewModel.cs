using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.UserDisplay;

namespace DragonCon.Features.Convention.Home
{
    public class HomeViewModel
    {
        public IList<DynamicSlideItem> CarouselItems = new List<DynamicSlideItem>();
    }
}
