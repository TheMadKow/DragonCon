using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Payment;
using NodaTime;

namespace DragonCon.Features.Participant.Personal
{
    public class PersonalViewModel
    {
        public EngagementWrapper MyEngagement { get; set; } = new EngagementWrapper();
        public List<EngagementWrapper> RelatedEngagements { get; set; } = new List<EngagementWrapper>();
    }
}
