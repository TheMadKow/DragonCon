using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;

namespace DragonCon.Modeling.Models.Events
{
    public interface IConEvent
    {
        string ActivityId { get; set; }
        AgeRestriction Age { get; set; }
        string Comments { get; set; }
        string ConventionDayId { get; set; }
        string Description { get; set; }
        string GameMasterId { get; set; }
        bool HasBeenRevised { get; set; }
        IList<string> HelperIds { get; set; }
        List<string> HistoryIds { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        List<string> ParticipantIds { get; set; }
        SizeRestriction Size { get; set; }
        EventStatus Status { get; set; }
        string SystemId { get; set; }
        string TableId { get; set; }
        List<string> Tags { get; set; }
        TimeSlot TimeSlot { get; set; }
    }
}