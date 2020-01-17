using System;
using System.Linq;
using DragonCon.Features.Management.Reception;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.RavenDB.Index;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Managements
{
    public class RavenManagementReceptionGateway : EngagementRavenGateway, IManagementReceptionGateway
    {
        public RavenManagementReceptionGateway(IServiceProvider provider) : base(provider)
        {
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
                .As<UserEngagement>()
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
