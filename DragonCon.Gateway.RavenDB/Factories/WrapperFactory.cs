using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Tickets;
using Raven.Client.Documents.Session;
using Serilog;

namespace DragonCon.RavenDB.Factories
{
    public class WrapperFactory
    {
        private IDocumentSession _session;
        public WrapperFactory(IDocumentSession session)
        {
            _session = session;
        }

        private void WarnIfNotLoaded(IEnumerable<string> entities)
        {
            foreach (var entity in entities)
            {
                WarnIfNotLoaded(entity);
            }
        }

        private void WarnIfNotLoaded(string entity)
        {
            if (entity != null && _session.Advanced.IsLoaded(entity) == false)
            {
                Log.Logger.Warning($"Please include '{entity}' in your session query or load before accessing {GetType().FullName}.");
            }
        }


        #region EventWrapper
        public List<EventWrapper> Wrap(IEnumerable<Event> items)
        {
            return items.Select(Wrap).ToList();
        }

        public EventWrapper Wrap(Event item)
        {
            WarnIfNotLoaded(item.ConventionDayId);
            WarnIfNotLoaded(item.ActivityId);
            if (item.SubActivityId.IsNotEmptyString())
                WarnIfNotLoaded(item.SubActivityId);
            WarnIfNotLoaded(item.GameHostIds);
            WarnIfNotLoaded(item.HallId);
            WarnIfNotLoaded(item.AgeId);

            return new EventWrapper(item)
            {
                Day = _session.Load<Day>(item.ConventionDayId),
                Activity = _session.Load<Activity>(item.ActivityId),
                SubActivity = item.SubActivityId.IsNotEmptyString()
                    ? _session.Load<Activity>(item.SubActivityId)
                    : Activity.General,
                GameHosts =
                    _session.Load<LongTermParticipant>(item.GameHostIds)
                        .Select(y => y.Value).ToList<IParticipant>(),
                Hall = _session.Load<Hall>(item.HallId),
                AgeGroup = _session.Load<AgeGroup>(item.AgeId)
            };
        }
        #endregion

        # region ConventionWrapper
        public List<ConventionWrapper> Wrap(IEnumerable<Convention> conventions)
        {
            return conventions.Select(Wrap).ToList();
        }

        public ConventionWrapper Wrap(Convention convention)
        {
            WarnIfNotLoaded(convention.HallIds);
            WarnIfNotLoaded(convention.TicketIds);
            WarnIfNotLoaded(convention.DayIds);
            var wrapper = new ConventionWrapper(convention)
            {
                Halls = _session.Load<Hall>(convention.HallIds).Select(x => x.Value).ToList(),
                Tickets = _session.Load<Ticket>(convention.TicketIds).Select(x => x.Value).ToList(),
                Days = _session.Load<Day>(convention.DayIds).Select(x => x.Value).ToList()
            };
            return wrapper;
        }
        #endregion

        #region EngagementWrapper
        private IParticipant GetLoadedParticipant(IUserEngagement x)
        {
            if (x.IsLongTerm)
                return _session.Load<LongTermParticipant>(x.ParticipantId);

            return _session.Load<ShortTermParticipant>(x.ParticipantId);
        }

        public List<EngagementWrapper> Wrap(IEnumerable<IUserEngagement> engagement, bool loadEvents = true)
        {
            return engagement.Select(x => Wrap(x, loadEvents)).ToList();
        }
        public EngagementWrapper Wrap(IUserEngagement engagement, bool loadEvents = true)
        {
            WarnIfNotLoaded(engagement.ParticipantId);
            WarnIfNotLoaded(engagement.ConventionId);

            if (loadEvents)
            {
                WarnIfNotLoaded(engagement.EventIds);
                WarnIfNotLoaded(engagement.SuggestedEventIds);
            }

            var events = new Dictionary<string, Event>();
            if (loadEvents)
            {
                var combinedEvents = new List<string>();
                combinedEvents.AddRange(engagement.EventIds);
                combinedEvents.AddRange(engagement.SuggestedEventIds);
                events = _session.Load<Event>(combinedEvents);
            }

            var wrapper = new EngagementWrapper(engagement as UserEngagement)
            {
                Participant = GetLoadedParticipant(engagement),
                Convention = _session.Load<Convention>(engagement.ConventionId),
                Events = events
                    .Where(x => engagement.EventIds.Contains(x.Key))
                    .Select(x => WrapJustDate(x.Value))
                    .ToList(),
                SuggestedEvents = events
                    .Where(x => engagement.SuggestedEventIds.Contains(x.Key))
                    .Select(x => WrapJustDate(x.Value))
                    .ToList(),
            };

            return wrapper;
        }

        private EngagedEvent WrapJustDate(Event item)
        {
            WarnIfNotLoaded(item.ConventionDayId);
            
            return new EngagedEvent(item)
            {
                Day = _session.Load<Day>(item.ConventionDayId),
            };
        }
        #endregion


    }
}
