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
using DragonCon.Modeling.Models.UserDisplay;
using DragonCon.Modeling.TimeSlots;
using DragonCon.RavenDB;
using DragonCon.RavenDB.Index;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Activity = DragonCon.Modeling.Models.Common.Activity;

namespace DragonCon.App.Helpers
{
    public class InitializeActorMiddleware
    {
        private readonly RequestDelegate _next;

        private const int CacheMinutes = 15;

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
            using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(CacheMinutes)))
            {
                var stopWatch = Stopwatch.StartNew();

                actor.System = LoadSystem(session);
                actor.Me = LoadMe(httpContext, session);

                if (actor.HasAnySystemRole)
                {
                    var managed = LoadConvention(actor.System.ManagersConventionId, session);
                    if (managed != null)
                    {
                        actor.HasManagedConvention = true;
                        actor.ManagedConvention = managed;
                        actor.ManagedDropDowns = LoadDropDowns(factory, actor.ManagedConvention, actor.System);
                    }
                }

                if (actor.HasManagedConvention &&
                    actor.System.DisplayConventionId == actor.System.ManagersConventionId)
                {
                    actor.HasDisplayConvention = true;
                    // Copy already created objects
                    actor.DisplayConvention = actor.ManagedConvention;
                    actor.DisplayDropDowns = actor.ManagedDropDowns;

                    // Still, Create dynamic content
                    actor.DisplayDynamics = LoadDynamicContent(session, actor.DisplayConvention.ConventionId);
                }
                else
                {
                    var display = LoadConvention(actor.System.DisplayConventionId, session);
                    if (display != null)
                    {
                        actor.HasDisplayConvention = true;
                        actor.DisplayConvention = display;
                        actor.DisplayDropDowns = LoadDropDowns(factory, actor.DisplayConvention, actor.System);
                        actor.DisplayDynamics = LoadDynamicContent(session, actor.DisplayConvention.ConventionId);
                    }

                }
                stopWatch.Stop();
                actor.LoadTimeInMs = stopWatch.ElapsedMilliseconds;
            }

            return _next(httpContext);
        }

        private Actor.ActorDynamicContent LoadDynamicContent(IDocumentSession session, string conventioId)
        {
            var dynamicContent = session
                .Query<DynamicContent_ByConventionId.Result, DynamicContent_ByConventionId>()
                .Include(x => x.Updates)
                .Include(x => x.Slides)
                .Include(x => x.Sponsors)
                .Include(x => x.English)
                .Include(x => x.Location)
                .Include(x => x.Linkage)
                .SingleOrDefault(x => x.ConventionId == conventioId);

            if (dynamicContent == null)
                throw new Exception("Failed to find dynamic content");
            else
            {

                return new Actor.ActorDynamicContent()
                {
                    English = session.Load<DynamicEnglish>(dynamicContent.English).Select(x => x.Value).FirstOrDefault(),
                    Linkage = session.Load<DynamicLinkage>(dynamicContent.Linkage).Select(x => x.Value).FirstOrDefault(),
                    Location = session.Load<DynamicLocation>(dynamicContent.Location).Select(x => x.Value).FirstOrDefault(),

                    Slides = session.Load<DynamicSlideItem>(dynamicContent.Slides).Select(x => x.Value).ToList(),
                    Sponsors = session.Load<DynamicSponsorItem>(dynamicContent.Sponsors).Select(x => x.Value).ToList(),
                    Updates = session.Load<DynamicUpdateItem>(dynamicContent.Updates).Select(x => x.Value).ToList(),
                };
            }

        }

        private Actor.ActorSystemState LoadSystem(IDocumentSession session)
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
                    TagLine = convention.TagLine,
                    TimeStrategy = convention.TimeStrategy,
                    Settings = convention.Settings,

                    Halls = halls.Where(x => x.Value != null).Select(x => x.Value).ToList(),
                    Days = days.Where(x => x.Value != null).Select(x => x.Value).ToList(),
                    Tickets = tickets.Where(x => x.Value != null).Select(x => x.Value).ToList()
                };

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
