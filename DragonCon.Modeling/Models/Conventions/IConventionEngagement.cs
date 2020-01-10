using System.Collections.Generic;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;
using NodaTime;

namespace DragonCon.Modeling.Models.Conventions
{
    public interface IConventionEngagement
    {
        string ConventionId { get; }
        string ParticipantId { get; }
        IPaymentInvoice Payment { get; }
        string CreatorId { get;}
        bool IsLongTerm { get; }
        List<string> EventIds { get; }
        List<string> SuggestedEventIds { get; }
        List<ConventionRoles> Roles { get; }
        Instant CreatedOn { get; }
    }
}