using System.Collections.Generic;

namespace DragonCon.Modeling.Models.HallsTables
{
    public class Hall
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Table> Tables { get; set; }
        public string Description { get; set; }
    }
}