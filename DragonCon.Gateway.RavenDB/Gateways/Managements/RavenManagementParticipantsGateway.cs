using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Features.Management.Participants;
using DragonCon.Features.Shared;
using DragonCon.Logical.Identities;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;
using DragonCon.RavenDB.Factories;
using DragonCon.RavenDB.Index;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Managements
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
                        result.YearOfBirth = longTerm.YearOfBirth;
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
                        result.YearOfBirth = shortTerm.YearOfBirth;
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
            var result = new UpdateRolesViewModel
            {
                ParticipantId = participantId
            };

            var engagement = Session.Query<UserEngagement>()
                .SingleOrDefault(x => x.ParticipantId == participantId &&
                                                     x.ConventionId == Actor.ManagedConvention.ConventionId);
            var roles = engagement != null ? engagement.Roles : new List<ConventionRoles>();
            if (participantId.StartsWith("LongTerm"))
            {
                var longTerm = Session.Load<LongTermParticipant>(participantId);
                if (longTerm != null)
                {
                    result.IsLongTerm = true;
                    result.ParticipantName = longTerm.FullName;
                    result.SystemRoles = longTerm.SystemRoles;
                    result.ConventionRoles = roles;
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
                    result.ConventionRoles = roles;
                    return result;
                }

            }


            throw new Exception("Unknown Me Term or Me not found");
        }

        public Answer UpdateRoles(string participantId, string description, string[] sysKeys, string[] conKeys)
        {
            LongTermParticipant asLongTerm = null;
            if (participantId.StartsWith("LongTerm"))
            {
                asLongTerm = Session.Load<LongTermParticipant>(participantId);
            }
            var isLongTerm = asLongTerm != null;

            var engagement = Session
                .Query<UserEngagement>()
                .FirstOrDefault(x =>
                    x.ConventionId == Actor.ManagedConvention.ConventionId &&
                    x.ParticipantId == participantId);

            if (engagement == null)
            {
                engagement = new UserEngagement()
                {
                    ConventionId = Actor.ManagedConvention.ConventionId,
                    ConventionStartDate = Actor.ManagedConvention.Days
                        .Min(x => x.Date)
                        .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    ParticipantId = participantId,
                    IsLongTerm = isLongTerm
                };
            }

            foreach (ConventionRoles conRole in Enum.GetValues(typeof(ConventionRoles)))
            {
                if (conKeys.Contains(conRole.ToString()))
                {
                    engagement.AddRole(conRole);
                }
                else
                {
                    engagement.RemoveRole(conRole);
                }
            }

            if (description.IsEmptyString())
            {
                engagement.RoleDescription = "סגל כנס";
            }
            else
            {
                engagement.RoleDescription = description;
            }

            Session.Store(engagement);
            if (isLongTerm)
            {
                foreach (SystemRoles system in Enum.GetValues(typeof(SystemRoles)))
                {
                    if (sysKeys.Contains(system.ToString()))
                    {
                        asLongTerm.AddRole(system);
                    }
                    else
                    {
                        asLongTerm.RemoveRole(system);
                    }
                }
            }

            Session.SaveChanges();
            return Answer.Success;

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
                    longTerm.IsAllowingPromotions = viewmodel.IsAllowingPromotions ?? false;
                }

                participant.FullName = viewmodel.FullName;
                participant.PhoneNumber = viewmodel.PhoneNumber ?? string.Empty;
                participant.YearOfBirth = viewmodel.YearOfBirth;

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
            UserEngagement engagement = new UserEngagement
            {
                ConventionId = Actor.ManagedConvention.ConventionId,
                ConventionStartDate = Actor.ManagedConvention.Days
                    .Min(x => x.Date)
                    .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Payment = new PaymentInvoice()
            };

            if (viewmodel.Email.IsEmptyString())
            {
                model = new ShortTermParticipant
                {
                    CreatedById = Actor.Me.Id
                };
            }
            else
            {
                model = new LongTermParticipant
                {
                    UserName = viewmodel.Email,
                    Email = viewmodel.Email,
                    IsAllowingPromotions = viewmodel.IsAllowingPromotions ?? false,
                };
                engagement.IsLongTerm = true;
            }

            model.FullName = viewmodel.FullName;
            model.PhoneNumber = viewmodel.PhoneNumber ?? string.Empty;
            model.YearOfBirth = viewmodel.YearOfBirth;

            var result = await Identities.AddNewParticipant(model);

            engagement.ParticipantId = model.Id;
            if (result.IsSuccess)
            {
                Session.Store(engagement);
                Session.SaveChanges();
            }

            if (result.IsSuccess && result.IsLongTerm)
            {
                await Hub.SendCreationPasswordAsync(model as LongTermParticipant, result.Token);
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
                return Answer.Error("No Me Name");

            if (viewmodel.YearOfBirth < (DateTime.Today.Year - 120) ||
                viewmodel.YearOfBirth > DateTime.Today.Year)
                return Answer.Error("Illegal Year of Birth");


            return Answer.Success;
        }

        public ParticipantsManagementViewModel BuildIndex(IDisplayPagination pagination,
            bool allowHistory = false,
            ParticipantsManagementViewModel.Filters? filters = null)
        {
            var currentConvention = Actor.ManagedConvention.ConventionId;
            var query = Session.Query<UserEngagement>()
                .Include(x => x.ParticipantId)
                .Include(x => x.EventIds)
                .AsQueryable();
            if (allowHistory == false)
            {
                query = query.Where(x => x.ConventionId == currentConvention);
            }

            var engagements = query.ToList();
            var wrapperFactory = new WrapperFactory(Session);

            var result = new ParticipantsManagementViewModel
            {
                filters = filters,
                Pagination = pagination,
                Engagements = wrapperFactory.Wrap(engagements)
            };

            return result;
        }


        public ParticipantsManagementViewModel BuildSearchIndex(IDisplayPagination pagination, bool allowHistory = false, string searchWords = "")
        {
            if (searchWords.IsEmptyString())
                return new ParticipantsManagementViewModel();

            var query = Session.Query<Participants_BySearchQuery.Result, Participants_BySearchQuery>()
                .Include(x => x.ParticipantId)
                .Include(x => x.ConventionId)
                .Statistics(out var stats)
                .Search(x => x.SearchText, searchWords).AsQueryable();

            if (allowHistory == false)
            {
                query = query.Where(x => x.ConventionId == Actor.ManagedConvention.ConventionId);
            }

            var results = query
                .OrderBy(x => x.FullName)
                .Skip(pagination.SkipCount)
                .Take(pagination.ResultsPerPage)
                .As<UserEngagement>()
                .ToList();

            var wrapperFactory = new WrapperFactory(Session);
            var viewModel = new ParticipantsManagementViewModel
            {
                Pagination = DisplayPagination.BuildForView(
                    stats.TotalResults,
                    pagination.SkipCount,
                    pagination.ResultsPerPage),
                Engagements = wrapperFactory.Wrap(results, false),
                filters = new ParticipantsManagementViewModel.Filters()
            };
            return viewModel;
        }

    }
}