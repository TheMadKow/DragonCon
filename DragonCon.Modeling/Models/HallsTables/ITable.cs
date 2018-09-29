namespace DragonCon.Modeling.Models.HallsTables
{
    public interface ITable
    {
        string Id { get; set; }
        string Name { get; set; }
        string Notes { get; set; }
    }
}