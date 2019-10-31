using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Events;using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;

namespace DragonCon.Modeling.Models.Identities
{
    public class FullParticipantWrapper : Wrapper<LongTermParticipant>
    {
        public List<ShortTermParticipant> ConventionLimitedTickets { get; set; }

        public List<ConventionRoles> ConventionsRoles { get; set; }
        public IPaymentInvoice ConventionInvoice { get; set; }
    }

    public class LimitedParticipantWrapper : Wrapper<ShortTermParticipant>
    {
        private LongTermParticipant CreatedBy { get; set; }
    }
}
