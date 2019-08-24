using System.Collections.Generic;

namespace DragonCon.Modeling.Models.Common
{
    public class Activity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        
        public bool IsSubActivity { get; set; }
        public List<Activity> SubActivities { get; set; } = new List<Activity>();
    }
}
