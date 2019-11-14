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
        public IPaymentInvoice ConventionInvoice { get; set; }

    }

    public class LongTermParticipantWrapper : ParticipantWrapper
    {
        public List<ConventionRoles> ConventionsRoles { get; set; }
        public LongTermParticipantWrapper(IParticipant participant) : base(participant)
        {
        }
    }

    public class ShortTermParticipantWrapper : ParticipantWrapper
    {
        private IParticipant CreatedBy { get; set; }

        public ShortTermParticipantWrapper(IParticipant participant) : base(participant)
        {
        }
    }
}
