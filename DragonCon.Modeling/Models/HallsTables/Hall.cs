using System.Collections.Generic;

namespace DragonCon.Modeling.Models.HallsTables
{
    public class Hall : IHall
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ITable> Tables { get; set; }
        public string Description { get; set; }
    }
}