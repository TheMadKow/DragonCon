﻿using System.Globalization;
using System.Text;
using DragonCon.Modeling.Helpers;
using NodaTime;

namespace DragonCon.Modeling.Models.Common
{
    public class TimeSlot
    {
        public LocalTime From { get; set; }
        public LocalTime To { get; set; }
        public Period Span => To - From;
        public double DurationInHours => Span.ToDuration().TotalHours;

        public string GetDescription()
        {
            var sb = new StringBuilder();
            sb.Append(From.ToString(DragonConstants.DEFAULT_TIME, CultureInfo.CurrentCulture));
            sb.Append($" ({Span.Hours} שעות)");
            return sb.ToString();
        }

    }
}
