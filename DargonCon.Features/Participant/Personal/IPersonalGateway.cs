using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.ViewModels;

namespace DragonCon.Features.Participant.Personal
{
    public interface IPersonalGateway : IGateway
    {
        Answer AddSuggestedEvent(SuggestEventViewModel viewmodel);
        PersonalViewModel BuildPersonalViewModel();
    }
}
