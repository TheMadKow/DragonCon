using System;
using System.Diagnostics;
using System.Linq;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.System;
using DragonCon.Modeling.Models.Tickets;
using Raven.Client.Documents.Session;
using Activity = DragonCon.Modeling.Models.Common.Activity;

namespace DragonCon.RavenDB.Gateways
{
    public abstract class RavenGateway : IDisposable
    {
        private readonly StoreHolder _holder;
        public SystemState SystemState { get; private set; }
        protected IActor Actor { get; private set; }

        private IDocumentSession _session;
        public IDocumentSession Session => _session;

        private SystemState LoadSystemState()
        {
            var stopwatch = Stopwatch.StartNew();
            using (_session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromDays(1)))
            {
                var activities = _session.Query<Activity>().ToList();
                var ageGroups = _session.Query<AgeGroup>().ToList();

                var config = _session
                           .Include<SystemConfiguration>(x => x.ActiveConventionId)
                           .Load<SystemConfiguration>(SystemConfiguration.Id) ?? new SystemConfiguration();

                var convention = new Convention();
                if (config.ActiveConventionId.IsNotEmptyString())
                {
                    convention = _session
                        .Include<Convention>(x => x.DayIds)
                        .Include<Convention>(x => x.HallIds)
                        .Include<Convention>(x => x.TicketIds)
                        .Load<Convention>(config.ActiveConventionId);
                }

                if (convention == null)
                    convention = new Convention();

                var halls = _session.Load<Hall>(convention.HallIds);
                var tickets = _session.Load<Ticket>(convention.TicketIds);
                var days = _session.Load<Day>(convention.DayIds);

                var result = new SystemState
                {
                    Configurations = config,

                    AgeGroups = ageGroups,
                    Activities = activities.Where(x => x.IsSubActivity == false).ToList(),

                    ActiveConvention = new SystemState.ActiveConventionState()
                    {
                        Id = convention.Id,
                        Name = convention.Name,
                        Halls = halls.Where(x => x.Value != null).Select(x => x.Value).ToList(),
                        Days = days.Where(x => x.Value != null).Select(x => x.Value).ToList(),
                        Tickets = tickets.Where(x => x.Value != null).Select(x => x.Value).ToList()
                    }
                };
                stopwatch.Stop();
                result.BuildMilliseconds = stopwatch.ElapsedMilliseconds;
                return result;
            }
        }

        protected RavenGateway(StoreHolder holder, IActor actor)
        {
            _holder = holder;
            Actor = actor;
            _session = _holder.Store.OpenSession();
            SystemState = LoadSystemState();
        }

        private void ReleaseUnmanagedResources()
        {
            _session?.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RavenGateway()
        {
            Dispose(false);
        }
    }
}
