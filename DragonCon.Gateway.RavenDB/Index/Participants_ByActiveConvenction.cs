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
    public class Participants_ByActiveConvention : AbstractMultiMapIndexCreationTask
    {

        public Participants_ByActiveConvention()
        {
            AddMap<ShortTermParticipant>(shorts => from s in shorts
                select new
                {
                    FullName = s.FullName,
                    ActiveConventionTerm = s.ActiveConventionTerm,
                    ActiveConventionRoles = s.ActiveConventionRoles
                });
            AddMap<LongTermParticipant>(longs => from l in longs
                select new
                {
                    FullName = l.FullName,
                    ActiveConventionTerm = l.ActiveConventionTerm,
                    ActiveConventionRoles = l.ActiveConventionRoles
                });
        }
    }


    public class Participants_BySearchQuery : AbstractMultiMapIndexCreationTask<Participants_BySearchQuery.Result>
    {
        public class Result
        {
            public string ActiveConventionTerm { get; set; } = string.Empty;
            public string SearchText { get; set; } = string.Empty;
            public string FullName { get; set; }
        }

        public Participants_BySearchQuery()
        {
            AddMap<ShortTermParticipant>(shorts => from s in shorts
                select new
                {
                    FullName = s.FullName,
                    ActiveConventionTerm = s.ActiveConventionTerm,
                    SearchText = $"{s.Id} {s.FullName} {s.PhoneNumber}",
                });
            AddMap<LongTermParticipant>(longs => from l in longs
                select new
                {
                    FullName = l.FullName,
                    ActiveConventionTerm = l.ActiveConventionTerm,
                    SearchText = $"{l.Id} {l.FullName} {l.PhoneNumber} {l.Email}",
                });

            Index("SearchText", FieldIndexing.Search);
            Analyze("SearchText", "StandardAnalyzer");
        }
    }
}
