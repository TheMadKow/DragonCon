using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Tickets;
using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Factories
{
    public class WrapperFactory
    {
        private IDocumentSession _session;
        public WrapperFactory(IDocumentSession session)
        {
            _session = session;
        }

        private void ThrowIfNotLoaded(IEnumerable<string> entities)
        {
            foreach (var entity in entities)
            {
                ThrowIfNotLoaded(entity);
            }
        }

        private void ThrowIfNotLoaded(string entity)
        {
            if (_session.Advanced.IsLoaded(entity) == false)
            {
                throw new Exception($"Please include '{entity}' in your query or load before accessing {GetType().FullName}.");
            }
        }


        #region EventWrapper
        public List<EventWrapper> Wrap(IEnumerable<Event> items)
        {
            return items.Select(Wrap).ToList();
        }

        public EventWrapper Wrap(Event item)
        {
            ThrowIfNotLoaded(item.ConventionDayId);
            ThrowIfNotLoaded(item.ActivityId);
            if (item.SubActivityId.IsNotEmptyString())
                ThrowIfNotLoaded(item.SubActivityId);
            ThrowIfNotLoaded(item.GameMasterIds);
            ThrowIfNotLoaded(item.HallId);
            ThrowIfNotLoaded(item.AgeId);
            
            return new EventWrapper(item)
            {
                Day = _session.Load<Day>(item.ConventionDayId),
                Activity = _session.Load<Activity>(item.ActivityId),
                SubActivity = item.SubActivityId.IsNotEmptyString()
                    ? _session.Load<Activity>(item.SubActivityId)
                    : Activity.General,
                GameMasters =
                    _session.Load<LongTermParticipant>(item.GameMasterIds)
                        .Select(y => y.Value).ToList<IParticipant>(),
                Hall = _session.Load<Hall>(item.HallId),
                AgeGroup = _session.Load<AgeGroup>(item.AgeId)
            };
        }
        #endregion

        #region ConventionWrapper



        #endregion

        #region EngagementWrapper
        public List<ConventionWrapper> Wrap(IEnumerable<Convention> conventions)
        {
            return conventions.Select(Wrap).ToList();
        }

        public ConventionWrapper Wrap(Convention convention)
        {
            ThrowIfNotLoaded(convention.HallIds);
            ThrowIfNotLoaded(convention.TicketIds);
            ThrowIfNotLoaded(convention.DayIds);
            var wrapper = new ConventionWrapper(convention)
            {
                Halls = _session.Load<Hall>(convention.HallIds).Select(x => x.Value).ToList(),
                Tickets = _session.Load<Ticket>(convention.TicketIds).Select(x => x.Value).ToList(),
                Days = _session.Load<Day>(convention.DayIds).Select(x => x.Value).ToList()
            };
            return wrapper;
        }
        #endregion

        #region ParticipantWrapper
        private IParticipant GetLoadedParticipant(IConventionEngagement x)
        {
            if (x.IsLongTerm)
                return _session.Load<LongTermParticipant>(x.ParticipantId);

            return _session.Load<ShortTermParticipant>(x.ParticipantId);
        }

        public List<ParticipantWrapper> Wrap(IEnumerable<IConventionEngagement> engagement)
        {
            return engagement.Select(Wrap).ToList();
        }

        public ParticipantWrapper Wrap(IConventionEngagement engagement)
        {
            ThrowIfNotLoaded(engagement.ParticipantId);
            ThrowIfNotLoaded(engagement.ConventionId);

            var participant = GetLoadedParticipant(engagement);
            ParticipantWrapper wrapper;
            if (engagement.IsLongTerm)
            {
                wrapper = new LongTermParticipantWrapper(participant);
            }
            else
            {
                wrapper = new ShortTermParticipantWrapper(participant);
            }

            if (engagement.ConventionId.IsNotEmptyString())
            {
                var convention = _session.Load<Convention>(engagement.ConventionId);
                wrapper.EngagedConventionId = engagement.ConventionId;
                wrapper.EngagedConventionInvoice = engagement.Payment;
                wrapper.EngagedConventionRoles = engagement.Roles;
                wrapper.EngagedConventionName = convention.Name;
            }

            return wrapper;
        }
        #endregion


    }
}
