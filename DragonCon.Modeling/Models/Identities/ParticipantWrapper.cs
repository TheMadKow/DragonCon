using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Events;using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;

namespace DragonCon.Modeling.Models.Identities
{
    public class ParticipantWrapper : Wrapper<IParticipant>
    {
        protected ParticipantWrapper(IParticipant participant) : base(participant)
        {

        }

        public string EngagedConventionId { get; set; }
        public string EngagedConventionName { get; set; }
        public List<ConventionRoles> EngagedConventionRoles { get; set; }
        public IPaymentInvoice EngagedConventionInvoice { get; set; }

    }

    public class LongTermParticipantWrapper : ParticipantWrapper
    {
        public LongTermParticipantWrapper(IParticipant participant) : base(participant)
        {
        }
    }

    public class ShortTermParticipantWrapper : ParticipantWrapper
    {
        public ShortTermParticipantWrapper(IParticipant participant) : base(participant)
        {
        }
    }
}
