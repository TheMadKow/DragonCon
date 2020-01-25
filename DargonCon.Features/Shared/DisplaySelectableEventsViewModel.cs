using System.Collections.Generic;

namespace DragonCon.Features.Shared
{
    public class DisplaySelectableEventsViewModel : DisplayEventsViewModel
    {
        public DisplaySelectableEventsViewModel()
        {
        }

        public List<string> EventIdsSelected = new List<string>();
        public bool AllowSelection { get; set; } = true;


        public string RegisterForId { get; set; } = string.Empty;
        public string RegisterForName { get; set; } = string.Empty;
    }
}