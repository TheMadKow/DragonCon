using System.Collections.Generic;

namespace DragonCon.Modeling.Models.HallsTables
{
    public interface IHall
    {
        string Description { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        List<ITable> Tables { get; set; }
    }
}