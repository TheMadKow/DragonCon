using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonCon.Modeling.Models.Tickets;

namespace DragonCon.Modeling.Models.Identities
{
    public interface ISystemUser
    {
        string EmailAddress { get; set; }
        List<string> Roles { get; set; } 
        bool IsAllowingPromotions { get; set; }
        List<PaymentInvoice> TicketPayments { get;set; }
    }
}
