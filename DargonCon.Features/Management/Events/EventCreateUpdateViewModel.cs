using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Logical.Factories;
using DragonCon.Modeling.Helpers;
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
        
        public string ActivitySelector { get;set; }
        public string ActivityId => ActivitySelector?.SplitTuples().Major ?? string.Empty;
        public string SubActivityId => ActivitySelector?.SplitTuples().Minor ?? string.Empty;

        public string HallTableSelector { get; set; }
        public string HallId => HallTableSelector?.SplitTuples().Major ?? string.Empty;
        public string HallTable => HallTableSelector?.SplitTuples().Minor ?? string.Empty;

        public List<string> GameMasterIds { get; set; }

        public EventStatus Status { get; set; }
        public SizeRestriction Size { get;set; }
        public string AgeId { get; set; }

        public string StartTimeSelector { get; set; }
        public string ConventionDayId => StartTimeSelector?.SplitTuples().Major ?? string.Empty;
        public string StartTime => StartTimeSelector?.SplitTuples().Minor ?? string.Empty;

        public double? Duration { get; set; }

        public List<string> Tags { get;set; }
        
        public string Description { get; set; }
        public string SpecialRequests { get;set; }

        public bool IsSpecialPrice { get; set; }
        public double? SpecialPrice { get; set; }
    }
}
