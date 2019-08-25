using System;
using System.Collections.Generic;
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

        public InitializeActorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, IActor actor, StoreHolder holder)
        {
            using (var session = holder.Store.OpenSession())
            {
                actor.State = LoadSystemState(session);
                actor.Participant = LoadParticipant(session);
            }

            return _next(httpContext);
        }

        private Actor.ParticipantActor LoadParticipant(IDocumentSession session)
        {
            return new Actor.ParticipantActor()
            {
                Id = "Test Admin User",
                SystemRoles =
                {
                    SystemRoles.ContentManager, SystemRoles.ConventionManager, SystemRoles.ReceptionStaff,
                    SystemRoles.UsersManager
                },
                FullName = "System Test User"
            };
        }

        private Actor.SystemState LoadSystemState(IDocumentSession _session)
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

                var result = new Actor.SystemState
                {
                    Configurations = config,

                    AgeGroups = ageGroups,
                    Activities = activities.Where(x => x.IsSubActivity == false).ToList(),

                    ActiveConvention = new Actor.SystemState.ActiveConventionState()
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
    }

    public static class InitializeActorMiddlewareExtensions
    {
        public static IApplicationBuilder UseActorInitialization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InitializeActorMiddleware>();
        }
    } 

}
