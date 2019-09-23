using System;
using System.Collections.Generic;
using System.Text;
using NodaTime;

namespace DragonCon.Modeling.Models.Common
{
    public class UserAction
    {
        public string DocumentId { get; set; }
        public Instant TimeStamp { get; set; }
    
        public string UserId { get; set; }
        public string Field { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
