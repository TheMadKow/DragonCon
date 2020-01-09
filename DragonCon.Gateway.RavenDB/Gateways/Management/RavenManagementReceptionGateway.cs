using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Features.Management.Participants;
using DragonCon.Features.Management.Reception;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.RavenDB.Index;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Management
{
    public class RavenManagementReceptionGateway : RavenGateway, IManagementReceptionGateway
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
                .Statistics(out var stats)
                .Search(x => x.SearchText, searchWords)
                .Where(x => x.IsLongTerm || x.ConventionTerm == conventionId)
                .Distinct()
                .OrderBy(x => x.FullName)
                .Skip(pagination.SkipCount)
                .Take(pagination.ResultsPerPage)
                .As<IConventionEngagement>()
                .ToList();

            var viewModel = new ParticipantsReceptionViewModel
            {
                Pagination = DisplayPagination.BuildForView(
                    stats.TotalResults,
                    pagination.SkipCount,
                    pagination.ResultsPerPage),
                Participants = query.Select(ParticipantWrapperBuilder).ToList(),
            };
            return viewModel;
        }
    }
}
