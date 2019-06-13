using System.Collections.Generic;

namespace DragonCon.Modeling.Models.HallsTables
{
    public class Hall : IHall
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int FirstTable { get; set; }
        public int LastTable { get; set; }
        public IList<int> Tables
        {
            get
            {
                var list = new List<int>();
                for (var i = FirstTable; i <= LastTable; i++)
                {
                    list.Add(i);
                }
                return list;
            }
        }
    }
}