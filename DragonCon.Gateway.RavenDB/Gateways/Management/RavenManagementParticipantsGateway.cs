using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Features.Management.Participants;
using DragonCon.Features.Shared;
using DragonCon.Logical.Communication;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
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

        public UpdateRolesViewModel GetRolesViewModel(string participantId)
        {
            var result = new UpdateRolesViewModel
            {
                ParticipantId = participantId
            };

            if (participantId.StartsWith("LongTerm"))
            {
                var longTerm = Session.Load<LongTermParticipant>(participantId);
                if (longTerm != null)
                {
                    result.IsLongTerm = true;
                    result.ParticipantName = longTerm.FullName;
                    result.ConventionRoles = longTerm.ActiveConventionRoles;
                    result.SystemRoles = longTerm.SystemRoles;
                    return result;
                }

            }

            if (participantId.StartsWith("ShortTerm"))
            {
                var shortTerm = Session.Load<ShortTermParticipant>(participantId);

                if (shortTerm != null)
                {
                    result.IsLongTerm = false;
                    result.ParticipantName = shortTerm.FullName;
                    result.ConventionRoles = shortTerm.ActiveConventionRoles;
                    return result;
                }

            }


            throw new Exception("Unknown Participant Term or Participant not found");
        }

        public Answer UpdateRoles(string participantId, string[] sysKeys, string[] conKeys)
        {
            IParticipant participant = null;
            if (participantId.StartsWith("ShortTerm"))
            {
                participant = Session.Load<ShortTermParticipant>(participantId);
            }

            if (participantId.StartsWith("LongTerm"))
            {
                participant = Session.Load<LongTermParticipant>(participantId);
            }

            if (participant != null)
            {
                foreach (ConventionRoles convention in Enum.GetValues(typeof(ConventionRoles)))
                {
                    if (conKeys.Contains(convention.ToString()))
                    {
                        participant.AddRole(convention);
                    }
                    else
                    {
                        participant.RemoveRole(convention);
                    }
                }

                if (participant is LongTermParticipant longTerm)
                {
                    foreach (SystemRoles system in Enum.GetValues(typeof(SystemRoles)))
                    {
                        if (sysKeys.Contains(system.ToString()))
                        {
                            longTerm.AddRole(system);
                        }
                        else
                        {
                            longTerm.RemoveRole(system);
                        }
                    }
                }

                Session.SaveChanges();
                return Answer.Success;
            }
            else
            {
                return Answer.Error("Couldn't find participant");
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


        public ParticipantsManagementViewModel BuildSearchIndex(IDisplayPagination pagination, string searchWords)
        {
            if (searchWords.IsEmptyString())
                return new ParticipantsManagementViewModel();

            var result = new ParticipantsManagementViewModel();
            var results = Session.Query<Participants_BySearchQuery.Result, Participants_BySearchQuery>()
                .Statistics(out var stats)
                .Search(x => x.SearchText, searchWords)
                .Where(x => x.ActiveConventionTerm == Actor.SystemState.ConventionId)
                .OrderBy(x => x.FullName)
                .Skip(pagination.SkipCount)
                .Take(pagination.ResultsPerPage)
                .As<IParticipant>()
                .ToList();

            var viewModel = new ParticipantsManagementViewModel
            {
                Pagination = DisplayPagination.BuildForView(
                    stats.TotalResults,
                    pagination.SkipCount,
                    pagination.ResultsPerPage),
                Participants = results.Select(ParticipantWrapperBuilder).ToList(),
                filters = new ParticipantsManagementViewModel.Filters()
            };
            return viewModel;
        }

    }
}