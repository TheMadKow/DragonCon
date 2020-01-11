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
        public List<EventWrapper> Events { get; set; } = new List<EventWrapper>();
        public List<EventWrapper> SuggestedEvents { get; set; } = new List<EventWrapper>();
    }
}