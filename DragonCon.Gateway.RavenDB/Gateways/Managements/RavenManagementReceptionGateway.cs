using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Features.Management.Participants;
using DragonCon.Features.Management.Reception;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;
using DragonCon.RavenDB.Index;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Managements
{
    public class RavenManagementReceptionGateway : RavenGateway, IManagementReceptionGateway
    {
        public RavenManagementReceptionGateway(IServiceProvider provider) : base(provider)
        {
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

        public async Task<Answer> AddParticipant(ParticipantCreateUpdateViewModel viewmodel)
        {
            var answer = ValidateParticipantFields(viewmodel);
            if (answer.AnswerType != AnswerType.Success)
                return answer;

            IParticipant model;
            ConventionEngagement engagement = new ConventionEngagement
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

            return new Answer(AnswerType.Success)
            {
                Message = model.Id
            };
        }

        public ParticipantsReceptionViewModel BuildParticipantSearch(IDisplayPagination pagination, string searchWords)
        {
            if (searchWords.IsEmptyString())
                return new ParticipantsReceptionViewModel();

            var conventionId = Actor.ManagedConvention.ConventionId;
            var query = Session
                .Query<Participants_BySearchQuery.Result, Participants_BySearchQuery>()
                .Include(x => x.ParticipantId)
                .Include(x => x.ConventionId)
                .Include(x => x.EventIds)
                .Statistics(out var stats)
                .Search(x => x.SearchText, searchWords)
                .Where(x => x.IsLongTerm || x.ConventionId == conventionId)
                .OrderBy(x => x.FullName)
                .Skip(pagination.SkipCount)
                .Take(pagination.ResultsPerPage)
                .As<ConventionEngagement>()
                .ToList();

            var wrapperFactory = new Factories.WrapperFactory(Session);
            var viewModel = new ParticipantsReceptionViewModel
            {
                Pagination = DisplayPagination.BuildForView(
                    stats.TotalResults,
                    pagination.SkipCount,
                    pagination.ResultsPerPage),
                Participants = wrapperFactory.Wrap(query)
            };
            return viewModel;
        }
    }
}
