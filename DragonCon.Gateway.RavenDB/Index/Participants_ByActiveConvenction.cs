using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.System;
using Raven.Client.Documents.Indexes;

namespace DragonCon.RavenDB.Index
{
    public class Participants_BySearchQuery : AbstractMultiMapIndexCreationTask<Participants_BySearchQuery.Result>
    {
        public class Result
        {
            public bool IsLongTerm { get; set; } = false;
            public string ConventionStartDate { get; set; } = string.Empty;
            public string ConventionId { get; set; } = string.Empty;
            public string SearchText { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string ParticipantId { get; set; } = string.Empty;
            public string[] EventIds { get; set; } = new string[0];
        }

        public Participants_BySearchQuery()
        {
            AddMap<ConventionEngagement>(shorts => from s in shorts
                where s.IsLongTerm == false
                      let shortTerm = LoadDocument<ShortTermParticipant>(s.ParticipantId)
                select new
                {
                    IsLongTerm = false,

                    EventIds = s.EventIds.Concat(s.SuggestedEventIds).ToList(),
                    FullName = shortTerm.FullName,
                    ParticipantId = s.ParticipantId,
                    ConventionId = s.ConventionId,
                    ConventionStartDate = s.ConventionStartDate,
                    SearchText = $"{shortTerm.Id} {shortTerm.FullName} {shortTerm.PhoneNumber}",
                });
            AddMap<ConventionEngagement>(longs => from l in longs
                where l.IsLongTerm
                let longTerm = LoadDocument<LongTermParticipant>(l.ParticipantId)
                select new
                {
                    IsLongTerm = true,
                    FullName = longTerm.FullName,
                    EventIds = l.EventIds.Concat(l.SuggestedEventIds).ToList(),
                    ParticipantId = l.ParticipantId,
                    ConventionId = l.ConventionId,
                    ConventionStartDate = l.ConventionStartDate,
                    SearchText = $"{longTerm.Id} {longTerm.FullName} {longTerm.PhoneNumber}",
                });

            Reduce = results => from result in results
                group result by result.ParticipantId
                into g
                let ordered = g.OrderByDescending(x => x.ConventionStartDate)
                let last = g.LastOrDefault()
                select last;

            Index("SearchText", FieldIndexing.Search);
            Analyze("SearchText", "StandardAnalyzer");
        }
    }
}
