using System;
using System.Collections.Generic;
using System.Text;

namespace DragonCon.Modeling.Models.UserDisplay
{
    public abstract class DynamicDisplayItem
    {
        public string Id { get; set; }
        public string ConventionId { get; set;} = string.Empty;
    }
}
