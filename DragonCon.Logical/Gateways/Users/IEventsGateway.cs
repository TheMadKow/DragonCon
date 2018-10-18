using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.ViewModels;

namespace DragonCon.Logical.Gateways.Users
{
    public interface IEventsGateway : IGateway
    {
        Answer AddSuggestedEvent(SuggestEventViewModel viewmodel);
    }
}
