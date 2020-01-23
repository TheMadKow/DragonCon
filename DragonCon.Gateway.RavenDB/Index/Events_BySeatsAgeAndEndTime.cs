using System.Linq;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using NodaTime;
using Raven.Client.Documents.Indexes;

namespace DragonCon.RavenDB.Index
{
    public class Events_BySeatsAgeAndEndTime : AbstractMultiMapIndexCreationTask<Events_BySeatsAgeAndEndTime.Result>
    {
        public class Result
        {
            public string ConventionId { get; set; } = string.Empty;
            public EventStatus Status { get; set; } = EventStatus.Pending;

            public string EventId { get; set; } = string.Empty;
            public string DayId { get; set; } = string.Empty;

            public LocalTime EndTime { get; set; }
            public LocalTime StartTime { get; set; }

            public int? MinAge { get; set; }
            public int? MaxAge { get; set; }


            public int SeatsCapacity { get; set; } = 0;
            public int SeatsTaken { get; set; } = 0;
            public int SeatsAvailable { get; set; } = 0;
        }


        public Events_BySeatsAgeAndEndTime()
        {
            AddMap<Event>(events =>
                from evnt in events
                let ageGroup = LoadDocument<AgeGroup>(evnt.AgeId)
                select new
                {
                    ConventionId = evnt.ConventionId,
                    EventId = evnt.Id,
                    DayId = evnt.ConventionDayId,
                    EndTime = evnt.TimeSlot.To,
                    StartTime = evnt.TimeSlot.From,
                    MinAge = ageGroup.MinAge,
                    MaxAge = ageGroup.MaxAge,
                    Status = evnt.Status,
                    SeatsCapacity = evnt.Size.Max,
                    SeatsTaken = 0,
                    SeatsAvailable = 0
                });

            AddMap<UserEngagement>(engagements
                => from conEngagement in engagements
                   from conEvent in conEngagement.EventIds
                   select new
                   {
                       ConventionId = conEngagement.ConventionId,
                       EventId = conEvent,
                       MinAge = 0,
                       MaxAge = 0,
                       DayId = "",
                       EndTime = "",
                       StartTime = "",
                       SeatsCapacity = "",
                       SeatsTaken = 1,
                       SeatsAvailable = 0,
                       Status = EventStatus.Approved,
                   });

            Reduce = results => from result in results
                                group result by result.EventId into g
                                let realFirst = g.FirstOrDefault(x => x.SeatsTaken == 0)
                                select new
                                {
                                    ConventionId = realFirst.ConventionId,
                                    Status = realFirst.Status,
                                    EventId = realFirst.EventId,
                                    MinAge = realFirst.MinAge,
                                    MaxAge = realFirst.MaxAge,
                                    DayId = realFirst.DayId,
                                    EndTime = realFirst.EndTime,
                                    StartTime = realFirst.StartTime,
                                    SeatsCapacity = realFirst.SeatsCapacity,
                                    SeatsTaken = g.Sum(x => x.SeatsTaken),
                                    SeatsAvailable = realFirst.SeatsCapacity - g.Sum(x => x.SeatsTaken)
                                };
        }
    }
}