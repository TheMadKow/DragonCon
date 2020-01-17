using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;

namespace DragonCon.Modeling.Models.Events
{
    public interface IEvent
    {
        string ActivityId { get; set; }
        string AgeId { get; set; }
        string SpecialRequests { get; set; }
        string ConventionDayId { get; set; }
        string Description { get; set; }
        List<string> GameHostIds { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        SizeRestriction Size { get; set; }
        EventStatus Status { get; set; }
        string SubActivityId { get; set; }
        string HallId { get; set; }
        int? HallTable { get; set; }
        TimeSlot TimeSlot { get; set; }

        bool IsFree { get; set; }
        double? ExtraCharge { get; set; }
    }
}