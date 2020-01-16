namespace DragonCon.Modeling.Models.UserDisplay
{
    public class DynamicLocation : DynamicDisplayItem
    {
        public string LocationName { get; set; } = string.Empty;
        public string LocationDescription { get; set; } = string.Empty;
        public string LocationWaysOfArrival { get; set; } = string.Empty;
        public string LocationMap { get; set; } = string.Empty;
        public string LocationImage { get; set; } = string.Empty;
    }
}