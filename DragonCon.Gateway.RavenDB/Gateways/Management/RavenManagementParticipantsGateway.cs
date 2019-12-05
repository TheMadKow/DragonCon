using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Features.Management.Participants;
using DragonCon.Features.Shared;
using DragonCon.Logical.Communication;
using DragonCon.Logical.Identities;
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
                var result = new ParticipantCreateUpdateViewModel
                {
                    Id = participantId,
                };

                if (participantId.StartsWith("LongTerm"))
                {
                    var longTerm = Session.Load<LongTermParticipant>(participantId);
                    if (longTerm != null)
                    {
                        result.DayOfBirth = longTerm.DayOfBirth.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture);
                        result.Email = longTerm.Email;
                        result.FullName = longTerm.FullName;
                        result.IsAllowingPromotions = longTerm.IsAllowingPromotions;
                        result.PhoneNumber = longTerm.PhoneNumber;
                        return result;
                    }

                }

                if (participantId.StartsWith("ShortTerm"))
                {
                    var shortTerm = Session.Load<ShortTermParticipant>(participantId);

                    if (shortTerm != null)
                    {
                        result.Email = string.Empty;
                        result.DayOfBirth = shortTerm.DayOfBirth.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture);
                        result.FullName = shortTerm.FullName;
                        result.PhoneNumber = shortTerm.PhoneNumber;
                        return result;
                    }
                }

                return result;
            }
        }

        public UpdateRolesViewModel GetRolesViewModel(string participantId)
        {
            var activeRoles = LoadConventionRolesContainer();
            
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
                    result.ConventionRoles = activeRoles.GetRolesForUser(participantId);
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
                    result.ConventionRoles = activeRoles.GetRolesForUser(participantId);
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
                var rolesContainer = LoadConventionRolesContainer();
                foreach (ConventionRoles conRole in Enum.GetValues(typeof(ConventionRoles)))
                {
                    if (conKeys.Contains(conRole.ToString()))
                    {
                        rolesContainer.AddRole(participantId, conRole);
                    }
                    else
                    {
                        rolesContainer.RemoveRole(participantId, conRole);
                    }
                }
                StoreConventionRolesContainer(rolesContainer);

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

        public async Task<Answer> UpdateParticipant(ParticipantCreateUpdateViewModel viewmodel)
        {
            var answer = ValidateParticipantFields(viewmodel);
            if (answer.AnswerType != AnswerType.Success)
                return answer;

            var participantId = viewmodel.Id;
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
                if (participant is LongTermParticipant longTerm)
                {
                    longTerm.IsAllowingPromotions = viewmodel.IsAllowingPromotions;
                }

                participant.FullName = viewmodel.FullName;
                participant.PhoneNumber = viewmodel.PhoneNumber ?? string.Empty;
                participant.DayOfBirth = CreateLocalDate(viewmodel.DayOfBirth);

                Session.Store(participant, participant.Id);
                Session.SaveChanges();

                return await Task.FromResult(Answer.Success);
            }
            else
            {
                return await Task.FromResult(Answer.Error("Can't find participant"));
            }
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

        public async Task<Answer> ResetPassword(string id)
        {
            var participant = Session.Load<LongTermParticipant>(id);
            if (participant != null)
            {
                var randomPassword = new RandomPasswordGenerator().Generate();
                var result = await Identities.SetPasswordAsync(participant, randomPassword);
                if (result.IsSuccess)
                {
                    await Hub.ResetParticipantPasswordAsync(participant, result.Token);
                    return Answer.Success;
                }
                return Answer.Error("Can't change password");
            }
            else 
                return Answer.Error("Can't find participant");
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
            bool allowHistory = false,
            ParticipantsManagementViewModel.Filters? filters = null)
        {
            var currentConvention = Actor.SystemState.ConventionId;
            var query = Session.Query<IParticipant, Participants_ByActiveConvention>().AsQueryable();
            if (allowHistory == false)
            {
                query = query.Where(x => x.ActiveConventionTerm == currentConvention);
            }

            var container = LoadConventionRolesContainer();
            var participants = query.ToList();
            var participantsUpgraded = participants.Select(x => ParticipantWrapperBuilder(x, container)).ToList();

            var result = new ParticipantsManagementViewModel
            {
                filters = filters,
                Pagination = pagination, 
                Participants = participantsUpgraded
            };
            return result;
        }

        private ParticipantWrapper ParticipantWrapperBuilder(IParticipant participant, ConventionRolesContainer container)
        {
            switch (participant)
            {
                case LongTermParticipant longTerm:
                    IPaymentInvoice lastPayment = null;
                    if (longTerm.ConventionAndPayment.ContainsKey(Actor.SystemState.ConventionId))
                        lastPayment = longTerm.ConventionAndPayment[Actor.SystemState.ConventionId];

                    return new LongTermParticipantWrapper(participant)
                    {
                        ActiveConventionInvoice = lastPayment,
                        ActiveConventionRoles = container.GetRolesForUser(participant.Id)
                    };
                case ShortTermParticipant shortTerm:
                    return new ShortTermParticipantWrapper(participant)
                    {
                        ActiveConventionInvoice = shortTerm.PaymentInvoice,
                        ActiveConventionRoles = container.GetRolesForUser(participant.Id)
                    };
            }

            return null;
        }


        public ParticipantsManagementViewModel BuildSearchIndex(IDisplayPagination pagination, bool allowHistory = false, string searchWords = "")
        {
            if (searchWords.IsEmptyString())
                return new ParticipantsManagementViewModel();

            var result = new ParticipantsManagementViewModel();
            var query = Session.Query<Participants_BySearchQuery.Result, Participants_BySearchQuery>()
                .Statistics(out var stats)
                .Search(x => x.SearchText, searchWords).AsQueryable();
            if (allowHistory == false)
            {
                query = query.Where(x => x.ActiveConventionTerm == Actor.SystemState.ConventionId);
            }

            var results = query
                .OrderBy(x => x.FullName)
                .Skip(pagination.SkipCount)
                .Take(pagination.ResultsPerPage)
                .As<IParticipant>()
                .ToList();
            
            var container = LoadConventionRolesContainer();
            var viewModel = new ParticipantsManagementViewModel
            {
                Pagination = DisplayPagination.BuildForView(
                    stats.TotalResults,
                    pagination.SkipCount,
                    pagination.ResultsPerPage),
                Participants = results.Select(x => ParticipantWrapperBuilder(x, container)).ToList(),
                filters = new ParticipantsManagementViewModel.Filters()
            };
            return viewModel;
        }

    }
}