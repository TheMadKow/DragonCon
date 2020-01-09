using System.Collections.Generic;
using DragonCon.Modeling.Models.System;
using DragonCon.Modeling.Models.UserDisplay;

namespace DragonCon.Features.Shared
{
    public class NullGateway : IGateway
    {
        public IList<DynamicSlideItem> CreateMockSlides()
        {
            var list = new List<DynamicSlideItem>();
            list.Add(new DynamicSlideItem()
            {
                Caption = "משחקי תפקידים",
                ImageUrl = "https://www.pundak.co.il/images/events/draco2019/IMG_8493.JPG"
            });
            list.Add(new DynamicSlideItem()
            {
                Caption = "צביעת מיניאטורות",
                ImageUrl = "https://www.pundak.co.il/images/events/draco2019/IMG_8514.JPG"
            });
            list.Add(new DynamicSlideItem()
            {
                Caption = "משחקי לוח",
                ImageUrl = "https://www.pundak.co.il/images/events/draco2019/IMG_8541.JPG"
            });

            list.Add(new DynamicSlideItem()
            {
                Caption = "משחקי מיניאטורות",
                ImageUrl = "https://www.pundak.co.il/images/events/draco2019/IMG_8650.JPG"
            });
            list.Add(new DynamicSlideItem()
            {
                Caption = "הדרכת משחקים",
                ImageUrl = "https://www.pundak.co.il/images/events/draco2019/IMG_8687.JPG"
            });
            list.Add(new DynamicSlideItem()
            {
                Caption = "צוות הקבלה",
                ImageUrl = "https://www.pundak.co.il/images/events/draco2019/IMG_8705.JPG"
            });
            list.Add(new DynamicSlideItem()
            {
                Caption = "משחקי הלוח",
                ImageUrl = "https://www.pundak.co.il/images/events/draco2019/IMG_8706.JPG"
            });
            list.Add(new DynamicSlideItem()
            {
                Caption = "משחקי התפקידים",
                ImageUrl = "https://www.pundak.co.il/images/events/draco2019/IMG_8424.JPG"
            });
            list.Add(new DynamicSlideItem()
            {
                Caption = "רוצים חולצה שלנו?",
                ImageUrl = "https://www.pundak.co.il/images/events/draco2019/IMG_8475.JPG"
            });
            list.Add(new DynamicSlideItem()
            {
                Caption = "מיניאטורות בקטנה",
                ImageUrl = "https://www.pundak.co.il/images/events/draco2019/IMG_8485.JPG"
            });
            return list;
        }
    }
}
