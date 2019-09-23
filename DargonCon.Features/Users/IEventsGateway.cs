using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.ViewModels;

namespace DragonCon.Features.Users
{
    public interface IEventsGateway : IGateway
    {
        Answer AddSuggestedEvent(SuggestEventViewModel viewmodel);
    }
}
