using System.Collections.Generic;

namespace DragonCon.Features.Shared
{
    public class DisplaySelectableEventsViewModel : DisplayEventsViewModel
    {
        public DisplaySelectableEventsViewModel()
        {
            ActiveFilters = new Filters
            {
                HideCompleted = true,
                HideTaken = true,
            };
        }

        public List<string> EventIdsSelected = new List<string>();
        public bool AllowSelection { get; set; } = true;
    }
}