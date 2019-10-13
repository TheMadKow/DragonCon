using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Logical.Factories;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.HallsTables;
using Microsoft.AspNetCore.Mvc.Rendering;
using NodaTime;

namespace DragonCon.Features.Management.Events
{
    public class EventCreateUpdateViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConventionDayId { get;set; }
        public string SystemId { get;set; }

        public List<string> GameMasterIds { get; set; }

        public EventStatus Status { get; set; }
        public SizeRestriction Size { get;set; }
        public string AgeRestrictionId { get; set; }

        public string StartTime { get; set; }
        public double? Duration { get; set; }

        public List<string> Tags { get;set; }
        public string Table { get; set; }
        
        public string Description { get; set; }
        public string SpecialRequests { get;set; }

        public bool IsSpecialPrice { get; set; }
        public double? SpecialPrice { get; set; }
    }
}
