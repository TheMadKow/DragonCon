using System;
using System.Collections.Generic;
using System.Text;

namespace DragonCon.Modeling.Models.UserDisplay
{
    public abstract class UserDisplayItem
    {
        public string Id { get; set; }
        public string ConventionId { get; set;} = String.Empty;
    }
}
