using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Events;

namespace DragonCon.Logic.Requests
{

    public class ManagerEventRequest : EventRequest
    {
        // TODO
    }

    public class EventRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string UserId { get; set; }
        public List<string> HelperIds { get; set; }
        public TimeSlot Timeslide { get; set; }
        public string DayId { get; set; }
        public string EventActivityId { get; set; }
        public EventRestriction Restrictions { get; set; }
        public string EventSystemId { get; set; }
        public List<string> Tags { get; set; }

        public ConventionEvent ToEvent()
        {
            return new ConventionEvent()
            {
                Description = this.Description,
                Comments = this.Comments,
                Name = this.Name,
                ConventionDayId = this.DayId,
                ActivityId = this.EventActivityId,
                ManagerId = this.UserId,
                HelpersId = this.HelperIds,
                SystemId = this.EventSystemId,
                Status = EventStatus.Pending,
                TimeSlot = this.Timeslide,
                Tags = this.Tags,
                Restrictions = this.Restrictions,
            };
        }
    }
}