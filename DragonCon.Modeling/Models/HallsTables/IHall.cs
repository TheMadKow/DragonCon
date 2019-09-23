using System.Collections.Generic;

namespace DragonCon.Modeling.Models.HallsTables
{
    public interface IHall
    {
        string Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }

        int FirstTable { get; set; }
        int LastTable { get; set; }
        IList<int> Tables { get; }
    }
}