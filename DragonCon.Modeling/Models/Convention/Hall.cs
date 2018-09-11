using System.Collections.Generic;

namespace DragonCon.Modeling.Models.Convention
{
    public class Hall
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Tables { get; set; }
        public string Description { get; set; }
    }
}