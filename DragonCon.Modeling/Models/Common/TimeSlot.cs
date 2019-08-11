using System.Globalization;
using System.Text;
using NodaTime;

namespace DragonCon.Modeling.Models.Common
{
    public class TimeSlot
    {
        public LocalTime From { get; set; }
        public LocalTime To { get; set; }
        public Period Span => To - From;

        public string GetDescription()
        {
            var sb = new StringBuilder();
            sb.Append(From.ToString("HH:mm", CultureInfo.CurrentCulture));
            sb.Append($" ({Span.Hours} שעות)");
            return sb.ToString();
        }

    }
}
