using System.Collections.Generic;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;
using Raven.Identity;

namespace DragonCon.Gateway.RavenDB.Identity
{
    public class RavenSystemUser : IdentityUser, ISystemUser
    {
        
        public string EmailAddress {get;set;}
        public new List<string> Roles {get;set;}
        public bool IsAllowingPromotions {get;set;}
        public List<PaymentInvoice> TicketPayments {get;set;}
    }
}
