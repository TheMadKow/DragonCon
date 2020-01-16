using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.UserDisplay;

namespace DragonCon.Features.Convention.Home
{
    public class AboutViewModel
    {
        public List<OfficerLine> OfficerLines = new List<OfficerLine>();
    }


    public class OfficerLine
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
