using System.Collections.Generic;

namespace DragonCon.Modeling.Models.Tickets
{
    public class Ticket
    {
        public string Id { get; set; }
        public List<string> DayIds { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string TransactionCode { get; set; }

        public int ActivitiesAllowed { get; set; }
        public bool UnlimitedActivities { get;set; }
    }
}