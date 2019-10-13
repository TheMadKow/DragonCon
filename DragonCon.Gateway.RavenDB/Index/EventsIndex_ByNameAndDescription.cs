using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.System;
using Raven.Client.Documents.Indexes;

namespace DragonCon.RavenDB.Index
{
    public class EventsIndex_ByTitleDescription : AbstractIndexCreationTask<Event, EventsIndex_ByTitleDescription.Result>
    {
        public class Result
        {
            public string EventId { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string SearchText { get; set; } = string.Empty;
            public string ConventionDayId { get; set; } = string.Empty;
            public string GameMasterIds { get; set; } = string.Empty;
            public string HallId { get; set; } = string.Empty;
            public string AgeId { get; set; } = string.Empty;
            public string ConventionId { get; set; } = string.Empty;
        }


        public EventsIndex_ByTitleDescription() : base()
        {
            Map = events => from conEvent in events
                select new
                {
                    EventId = conEvent.Id,
                    Name = conEvent.Name,
                    Description = conEvent.Description,
                    SearchText = $"{conEvent.Id} {conEvent.Name} {conEvent.Description}", 

                    ConventionDayId = conEvent.ConventionDayId,
                    ConventionId = conEvent.ConventionId,
                    GameMasterIds = conEvent.GameMasterIds,
                    HallId = conEvent.HallId,
                    AgeId = conEvent.AgeId
                };

            Index("SearchText", FieldIndexing.Search);
            Analyze("SearchText", "StandardAnalyzer");
        }
    }
}
