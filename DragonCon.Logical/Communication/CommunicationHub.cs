using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities;

namespace DragonCon.Logical.Communication
{
    public interface ICommunicationHub
    {
        Task<Answer> SendCreationPasswordAsync(IParticipant participant, string password);
        Task<Answer> ResetParticipantPasswordAsync(IParticipant participant, string password);
    }

    public class CommunicationHub : ICommunicationHub
    {
        public Task<Answer> SendCreationPasswordAsync(IParticipant participant, string password)
        {
            return Task.FromResult(Answer.Success);
        }

        public Task<Answer> ResetParticipantPasswordAsync(IParticipant participant, string password)
        {
            return Task.FromResult(Answer.Success);
        }
    }
}
