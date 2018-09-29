using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Tickets;
using Raven.Identity;

namespace DragonCon.Gateway.Identity
{
    public class RavenSystemUser : IdentityUser, ISystemUser
    {
        
        public string EmailAddress {get;set;}
        public new List<string> Roles {get;set;}
        public bool IsAllowingPromotions {get;set;}
        public List<PaymentInvoice> TicketPayments {get;set;}
    }
}
