namespace DragonCon.Modeling.Models.Common
{
    public class AgeRestriction
    {
        public string Id { get; set; }
        public int? MinAge { get;set; }
        public int? MaxAge { get;set; }
        public string Name { get; set; }
    }
}