using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Features.Management.Participants;
using DragonCon.Features.Shared;
using DragonCon.Logical.Communication;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;
using DragonCon.RavenDB.Index;
using NodaTime;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Management
{
    public class RavenManagementParticipantsGateway : RavenGateway, IManagementParticipantsGateway
    {
        public RavenManagementParticipantsGateway(IServiceProvider provider) :
            base(provider)
        {
        }
        
        public ParticipantCreateUpdateViewModel GetParticipantViewModel(string participantId)
        {
            if (participantId.IsEmptyString())
            {
                return new ParticipantCreateUpdateViewModel();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public Task<Answer> UpdateParticipant(ParticipantCreateUpdateViewModel viewmodel)
        {

            throw new NotImplementedException();
        }
        
        public async Task<Answer> CreateParticipant(ParticipantCreateUpdateViewModel viewmodel)
        {
            var answer = ValidateParticipantFields(viewmodel);
            if (answer.AnswerType != AnswerType.Success)
                return answer;

            IParticipant model;
            if (viewmodel.Email.IsEmptyString())
            {
                model = new ShortTermParticipant
                {
                    ActiveConventionTerm = Actor.SystemState.ConventionId,
                    CreatedById = Actor.Participant.Id
                };
            }
            else
            {
                model = new LongTermParticipant
                {
                    ActiveConventionTerm = Actor.SystemState.ConventionId,
                    UserName = viewmodel.Email,
                    Email = viewmodel.Email,
                    IsAllowingPromotions = viewmodel.IsAllowingPromotions,
                    ConventionAndPayment = new Dictionary<string, PaymentInvoice>()
                };
            }

            model.FullName = viewmodel.FullName;
            model.PhoneNumber = viewmodel.PhoneNumber ?? string.Empty;
            model.DayOfBirth = CreateLocalDate(viewmodel.DayOfBirth);

            var result = await Identities.AddNewParticipant(model);
            if (result.IsSuccess && result.IsLongTerm)
            {
                await Hub.SendCreationPasswordAsync(model, result.Token);
            }

            if (result.IsSuccess == false)
                return Answer.Error(result.Errors?.AsJson());

            return Answer.Success;
        }

        private Answer ValidateParticipantFields(ParticipantCreateUpdateViewModel viewmodel)
        {
            if (viewmodel.FullName.IsEmptyString())
                return Answer.Error("No Participant Name");
       
            if (viewmodel.DayOfBirth.IsEmptyString())
                return Answer.Error("No Date of Birth");


            return Answer.Success;
        }

        private LocalDate CreateLocalDate(string dob)
        {
            var parsedDate = dob.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            return new LocalDate(
                int.Parse(parsedDate[0]),
                int.Parse(parsedDate[1]),
                int.Parse(parsedDate[2]));
        }

        public ParticipantsManagementViewModel BuildIndex(IDisplayPagination pagination,
            ParticipantsManagementViewModel.Filters? filters = null)
        {
            var currentConvention = Actor.SystemState.ConventionId;
            var participants = Session.Query<IParticipant, Participants_ByActiveConvention>()
                .Where(x => x.ActiveConventionTerm == currentConvention)
                .ToList();

            var participantsUpgraded = participants.Select(ParticipantWrapperBuilder).ToList();

            var result = new ParticipantsManagementViewModel
            {
                filters = filters,
                Pagination = pagination, 
                Participants = participantsUpgraded
            };
            return result;
        }

        private ParticipantWrapper ParticipantWrapperBuilder(IParticipant participant)
        {
            switch (participant)
            {
                case LongTermParticipant longTerm:
                    IPaymentInvoice lastPayment = null;
                    if (longTerm.ConventionAndPayment.ContainsKey(Actor.SystemState.ConventionId))
                        lastPayment = longTerm.ConventionAndPayment[Actor.SystemState.ConventionId];

                    return new LongTermParticipantWrapper(participant)
                    {
                        ConventionInvoice = lastPayment,
                        //ConventionsRoles = longTerm.role
                    };
                case ShortTermParticipant shortTerm:
                    return new ShortTermParticipantWrapper(participant)
                    {
                        ConventionInvoice = shortTerm.PaymentInvoice
                    };
            }

            return null;
        }


        public ParticipantsManagementViewModel BuildIndex(IDisplayPagination pagination, string searchWords)
        {
            if (searchWords.IsEmptyString())
                return new ParticipantsManagementViewModel();

            throw new NotImplementedException();
            
            //var result = new ParticipantsManagementViewModel();
            //var results = Session.Query<EventsIndex_ByTitleDescription.Result, EventsIndex_ByTitleDescription>()
            //    .Statistics(out var stats)
            //    .Search(x => x.SearchText, searchWords)
            //    .Include(x => x.ConventionDayId)
            //    .Include(x => x.GameMasterIds)
            //    .Include(x => x.HallId)
            //    .Include(x => x.AgeId)
            //    .Where(x => x.ConventionId == Actor.SystemState.ConventionId)
            //    .OrderBy(x => x.Name)
            //    .Skip(pagination.SkipCount)
            //    .Take(pagination.ResultsPerPage)
            //    .As<Event>()
            //    .ToList();

        }

    }
}