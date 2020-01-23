using System.Threading.Tasks;
using DragonCon.Features.Participant.Account;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities;

namespace DragonCon.Features.Participant.Personal
{
    public interface IPersonalGateway : IGateway
    {
        Answer AddSuggestedEvent(SuggestEventViewModel viewmodel);
        PersonalViewModel BuildPersonalViewModel();
        LongTermParticipant GetParticipant(string id);

        // Account
        Task<Answer> ChangePassword(PasswordChangeViewModel viewmodel);
        Task<Answer> UpdateDetails(DetailsUpdateViewModel viewModel);

        // Register Events
        DisplaySelectableEventsViewModel BuildEvents(string forUserId);
    }
}
