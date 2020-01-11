using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.System;
using DragonCon.Modeling.Models.Tickets;
using DragonCon.Modeling.TimeSlots;
using DragonCon.RavenDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents.Session;
using Activity = DragonCon.Modeling.Models.Common.Activity;

namespace DragonCon.App.Helpers
{
    public class InitializeActorMiddleware
    {
        private readonly RequestDelegate _next;
        private int CacheDays = 1;

        public InitializeActorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, 
            IActor actor, 
            StoreHolder holder,
            IStrategyFactory factory)
        {
            using (var session = holder.Store.OpenSession())
            {
                actor.System = LoadSystem(session);
                actor.Me = LoadMe(httpContext, session);

                if (actor.HasSystemRole(SystemRoles.ContentManager) ||
                    actor.HasSystemRole(SystemRoles.ConventionManager) ||
                    actor.HasSystemRole(SystemRoles.ReceptionManager) ||
                    actor.HasSystemRole(SystemRoles.UsersManager))
                {
                    actor.ManagedConvention = LoadConvention(actor.System.ManagersConventionId, session);
                    if (actor.ManagedConvention != null)
                        actor.ManagedDropDowns = LoadDropDowns(factory, actor.ManagedConvention, actor.System);
                }

                if (actor.ManagedConvention != null &&
                    actor.System.DisplayConventionId == actor.System.ManagersConventionId)
                {
                    actor.DisplayConvention = actor.ManagedConvention;
                    actor.DisplayDropDowns = actor.ManagedDropDowns;
                }
                else
                {
                    actor.DisplayConvention = LoadConvention(actor.System.DisplayConventionId, session);
                    if (actor.DisplayConvention != null)
                        actor.DisplayDropDowns = LoadDropDowns(factory, actor.DisplayConvention, actor.System);
                }
            }

            return _next(httpContext);
        }

        private Actor.ActorSystemState LoadSystem(IDocumentSession session)
        {
            using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromDays(CacheDays)))
            {
                var system = session
                    .Include<SystemConfiguration>(x => x.ManagedConventionId)
                    .Include<SystemConfiguration>(x => x.DisplayConventionId)
                    .Load<SystemConfiguration>(SystemConfiguration.Id);

                var activities = session.Query<Activity>().ToList()
                    .Where(x => x.IsSubActivity == false).ToList();
                var ageGroups = session.Query<AgeGroup>().ToList();

                if (system == null)
                {
                    system = new SystemConfiguration();
                }

                return new Actor.ActorSystemState
                {
                    ManagersConventionId = system.ManagedConventionId,
                    DisplayConventionId = system.DisplayConventionId,
                    Activities = activities,
                    AgeGroups = ageGroups
                };
            }
        }

        private Actor.ActorDropDowns LoadDropDowns(IStrategyFactory factory, Actor.ActorConventionState convention, Actor.ActorSystemState system)
        {
            return new Actor.ActorDropDowns(factory, convention, system);
        }

        private Actor.ActorParticipant LoadMe(HttpContext httpContext, IDocumentSession session)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var user = session.Query<LongTermParticipant>()
                    .FirstOrDefault(x => x.Email == httpContext.User.Identity.Name);
                if (user != null)
                {
                    return new Actor.ActorParticipant
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        SystemRoles = user.SystemRoles
                    };
                }
            }

            return new Actor.ActorParticipant();

        }

        private Actor.ActorConventionState? LoadConvention(string conventionId, IDocumentSession _session)
        {
            var stopwatch = Stopwatch.StartNew();
            using (_session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromDays(1)))
            {

                if (conventionId.IsEmptyString())
                    return null;

                var convention = _session
                        .Include<Convention>(x => x.DayIds)
                        .Include<Convention>(x => x.HallIds)
                        .Include<Convention>(x => x.TicketIds)
                        .Load<Convention>(conventionId);

                if (convention == null)
                    return null;

                var halls = _session.Load<Hall>(convention.HallIds);
                var tickets = _session.Load<Ticket>(convention.TicketIds);
                var days = _session.Load<Day>(convention.DayIds);

                var result = new Actor.ActorConventionState
                {
                    ConventionId = convention.Id,
                    ConventionName = convention.Name,
                    Location = convention.Location,
                    TagLine = convention.TagLine,
                    TimeStrategy = convention.TimeStrategy,
                    Settings = convention.Settings,
              
                    Halls = halls.Where(x => x.Value != null).Select(x => x.Value).ToList(),
                    Days = days.Where(x => x.Value != null).Select(x => x.Value).ToList(),
                    Tickets = tickets.Where(x => x.Value != null).Select(x => x.Value).ToList()
                };
                
                stopwatch.Stop();
                result.BuildMilliseconds = stopwatch.ElapsedMilliseconds;
                return result;
            }
        }
    }

    public static class InitializeActorMiddlewareExtensions
    {
        public static IApplicationBuilder UseActorInitialization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InitializeActorMiddleware>();
        }
    } 

}
