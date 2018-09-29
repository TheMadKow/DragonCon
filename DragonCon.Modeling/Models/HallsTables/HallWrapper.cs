using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;

namespace DragonCon.Modeling.Models.HallsTables
{
    public class HallWrapper : Wrapper<Hall>
    {
        public HallWrapper()
        {
            Model = new Hall();
        }
        public HallWrapper(Hall model) : base(model) { }

        public string Id { get => Model.Id; set => Model.Id = value; }
        public string Name { get => Model.Name; set => Model.Name = value; }
        public List<ITable> Tables { get => Model.Tables; set => Model.Tables = value; }
        public string Description { get => Model.Description; set => Model.Description = value; }
    }
}