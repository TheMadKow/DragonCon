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
            public string ConventionTerm { get; set; } = string.Empty;
            public string SearchText { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string ParticipantId { get; set; } = string.Empty;
        }

        public Participants_BySearchQuery()
        {
            AddMap<ConventionEngagement>(shorts => from s in shorts
                where s.IsLongTerm == false
                      let shortTerm = LoadDocument<ShortTermParticipant>(s.ParticipantId)
                select new
                {
                    IsLongTerm = false,
                    FullName = shortTerm.FullName,
                    ParticipantId = s.ParticipantId,
                    ConventionTerm = s.ConventionId,
                    SearchText = $"{shortTerm.Id} {shortTerm.FullName} {shortTerm.PhoneNumber}",
                });
            AddMap<ConventionEngagement>(longs => from l in longs
                where l.IsLongTerm
                let longTerm = LoadDocument<LongTermParticipant>(l.ParticipantId)
                select new
                {
                    IsLongTerm = true,
                    FullName = longTerm.FullName,
                    ParticipantId = l.ParticipantId,
                    ConventionTerm = l.ConventionId,
                    SearchText = $"{longTerm.Id} {longTerm.FullName} {longTerm.PhoneNumber}",
                });

            Index("SearchText", FieldIndexing.Search);
            Analyze("SearchText", "StandardAnalyzer");
        }
    }
}
