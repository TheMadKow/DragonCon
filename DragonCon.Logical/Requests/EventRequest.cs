using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Events;

namespace DragonCon.Logical.Requests
{

    public class ManagerEventRequest : EventRequest
    {
        // TODO
    }

    public class EventRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Requests { get; set; }
        public string UserId { get; set; }
        public List<string> HelperIds { get; set; }
        public TimeSlot Timeslide { get; set; }
        public string DayId { get; set; }
        public string EventActivityId { get; set; }
        
        public AgeRestriction AgeRestrictions { get; set; }
        public SizeRestriction SizeRestrictions { get; set; }

        public string EventSystemId { get; set; }
        public List<string> Tags { get; set; }

        public ConEvent ToEvent()
        {
            return new ConEvent()
            {
                Description = this.Description,
                Requests = this.Requests,
                Name = this.Name,
                ConventionDayId = this.DayId,
                ActivityId = this.EventActivityId,
                GameMasterId = this.UserId,
                HelperIds = this.HelperIds,
                SystemId = this.EventSystemId,
                Status = EventStatus.Pending,
                TimeSlot = this.Timeslide,
                Tags = this.Tags,
                Size = this.SizeRestrictions,
                Age = this.AgeRestrictions
            };
        }
    }
}