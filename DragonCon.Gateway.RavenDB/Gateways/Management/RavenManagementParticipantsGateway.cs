using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonCon.Features.Management.Participants;
using DragonCon.Features.Shared;
using DragonCon.Logical.Communication;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;
using NodaTime;

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
                    ConventionIdTerm = Actor.SystemState.ConventionId,
                    CreatedById = Actor.Participant.Id
                };
            }
            else
            {
                model = new LongTermParticipant
                {
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

        public ParticipantsManagementViewModel BuildIndex(IDisplayPagination pagination,
            ParticipantsManagementViewModel.Filters? filters = null)
        {
            var result = new ParticipantsManagementViewModel();
            result.filters = filters;
            result.Pagination = pagination;
            result.Participants = new List<ParticipantWrapper>();
            return result;
        }

    }
}