using System.Threading.Tasks;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities;

namespace DragonCon.Logical.Communication
{
    public interface ICommunicationHub
    {
        Task<Answer> SendCreationPasswordAsync(LongTermParticipant participant, string password);
        Task<Answer> ResetParticipantPasswordAsync(LongTermParticipant participant, string password);
        Task<Answer> SendWelcomeMessageAsync(LongTermParticipant participant);
        Task<Answer> SendForgotPassword(LongTermParticipant user, string callbackUrl);
    }

    public class CommunicationHub : ICommunicationHub
    {
        public Task<Answer> SendCreationPasswordAsync(LongTermParticipant participant, string password)
        {
            return Task.FromResult(Answer.Success);
        }

        public Task<Answer> ResetParticipantPasswordAsync(LongTermParticipant participant, string password)
        {
            return Task.FromResult(Answer.Success);
        }

        public Task<Answer> SendWelcomeMessageAsync(LongTermParticipant participant)
        {
            return Task.FromResult(Answer.Success);
        }

        public Task<Answer> SendForgotPassword(LongTermParticipant user, string callbackUrl)
        {
            return Task.FromResult(Answer.Success);
        }
    }
}
