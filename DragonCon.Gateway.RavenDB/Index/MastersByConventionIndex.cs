using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.System;
using DragonCon.RavenDB.Identity;
using Raven.Client.Documents.Indexes;

namespace DragonCon.RavenDB.Index
{
    public class MastersByConventionIndex : AbstractIndexCreationTask<ConEvent, MastersByConventionIndex.Result>
    {
        public class Result
        {
            public string GameMasterId { get; set; }
            public string GameMasterName { get; set; }
            public string ConventionId { get; set; }
            
        }

        public MastersByConventionIndex()
        {
            Map = events => from anEvent in events
                let user = LoadDocument<RavenSystemUser>(anEvent.GameMasterId)
                select new
                {
                    GameMasterName = $"{user.FirstName} {user.LastName}",
                    GameMasterId = anEvent.GameMasterId,
                    ConventionId = anEvent.ConventionId
                };
            
            Reduce = results => from result in results
                group result by new {result.ConventionId, result.GameMasterId} into groupedMasters
                select new
                {
                    GameMasterName = groupedMasters.First().GameMasterName,
                    GameMasterId = groupedMasters.First().GameMasterId,
                    ConventionId = groupedMasters.First().ConventionId
                };

        }
    }
}
