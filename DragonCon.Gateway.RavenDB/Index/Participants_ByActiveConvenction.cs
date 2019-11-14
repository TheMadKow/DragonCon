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
                    ActiveConventionTerm = s.ActiveConventionTerm
                });
            AddMap<LongTermParticipant>(longs => from l in longs
                select new
                {
                    FullName = l.FullName,
                    ActiveConventionTerm = l.ActiveConventionTerm
                });
        }
    }
}
