using System.Globalization;
using System.Text;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using NodaTime;

namespace DragonCon.Modeling.Models.Conventions
{
    public class Day
    {
        public Day() { }
        public Day(LocalDate date, LocalTime start, LocalTime end)
        {
            Date = date;
            StartTime = start;
            EndTime = end;
        }

        public string Id { get;set; }
        public LocalDate Date { get; set; }
        public LocalTime StartTime { get; set; }
        public LocalTime EndTime { get; set; }
        public TimeSlotStrategy TimeSlotStrategy { get; set; }

        public string GetDescription()
        {
            var sb = new StringBuilder();
            sb.Append(Date.DayOfWeek.InHebrew());
            sb.Append(Date.ToString($" ה-{DragonConstants.DEFAULT_DATE}", CultureInfo.DefaultThreadCurrentCulture));
            return sb.ToString();
        }
    }
}