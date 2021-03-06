﻿using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using NodaTime;

namespace DragonCon.Modeling.Models.Events
{
    public class Event : IEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConventionId { get; set; }
        public string ConventionDayId { get;set; }

        public string ActivityId { get; set; }
        public string SubActivityId { get;set; }

        public List<string> GameMasterIds { get; set; }

        public string AgeId { get; set; }
        public EventStatus Status { get; set; }
        public TimeSlot TimeSlot { get;set; }
        public bool IsSpecialPrice { get; set; }
        public double? SpecialPrice { get; set; }
        public SizeRestriction Size { get;set; }

        public List<string> Tags { get;set; }
        public string HallId { get; set; }
        public int? HallTable { get; set; }
        
        public string Description { get; set; }
        public string SpecialRequests { get;set; }

        public Instant CreatedOn { get;set; }
        public Instant UpdatedOn { get;set; }
    }
}
