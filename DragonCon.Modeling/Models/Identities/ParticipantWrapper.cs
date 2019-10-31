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
        public string Id { get; set; }
        public IPaymentInvoice ConventionInvoice { get; set; }

    }

    public class LongTermParticipantWrapper : Wrapper<ParticipantWrapper>
    {
        public List<ConventionRoles> ConventionsRoles { get; set; }
        public List<SystemRoles> SystemRoles { get; set; }
    }

    public class ShortTempParticipantWrapper : Wrapper<ParticipantWrapper>
    {
        private IParticipant CreatedBy { get; set; }
    }
}
