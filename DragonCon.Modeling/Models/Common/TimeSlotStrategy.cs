using System.ComponentModel;

namespace DragonCon.Modeling.Models.Common
{
    public enum TimeSlotStrategy
    {
        // ReSharper disable once InconsistentNaming
        [Description("התחלה כל שעתיים, חלונות זמן של 2/4/6 שעות")]
        StartEvery2Hours_Duration246Windows,

        [Description("התחלה כל שעתיים, חלונות זמן של 1.5/3.5/5.5 שעות")]
        StartEvery2Hours_Duration90MinutesWindows,

    }
}
