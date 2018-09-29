namespace DragonCon.Modeling.Models.HallsTables
{
    public class Table
    {
        public Table(string hallId, string name)
        {
            Id = $"Halls/{hallId}/Tables/{name}";
        }

        public string Id {get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
    }
}