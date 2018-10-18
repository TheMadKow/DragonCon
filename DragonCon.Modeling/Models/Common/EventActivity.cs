using System.Collections.Generic;

namespace DragonCon.Modeling.Models.Common
{
    public class EventActivity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<EventSystem> ActivitySystems { get; set; }
    }
}
