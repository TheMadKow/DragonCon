using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.Identities;

namespace DragonCon.Modeling.Models.Conventions
{
    public class EngagementWrapper : Wrapper<ConventionEngagement>
    {
        public EngagementWrapper()
        {

        }
        public EngagementWrapper(IConventionEngagement engagement)
        {

        }

        public IParticipant Participant { get; set; }
        public Convention Convention { get; set; }
        public List<EngagedEvent> Events { get; set; } = new List<EngagedEvent>();
        public List<EngagedEvent> SuggestedEvents { get; set; } = new List<EngagedEvent>();
    }

    public class EngagedEvent
    {
        public Event Event;

        public EngagedEvent(Event item)
        {
            Event = item;
        }

        public Day Day { get; set; }
    }
}