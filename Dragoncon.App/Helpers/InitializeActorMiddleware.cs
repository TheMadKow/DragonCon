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
                actor.Participant = LoadParticipant(session);
                actor.SystemState = LoadSystemState(session);
                actor.DropDowns = LoadDropDowns(factory, actor.SystemState);
            }

            return _next(httpContext);
        }

        private Actor.ActorDropDowns LoadDropDowns(IStrategyFactory factory, Actor.ActorSystemState actorSystemState)
        {
            return new Actor.ActorDropDowns(factory, actorSystemState);
        }

        private Actor.ActorParticipant LoadParticipant(IDocumentSession session)
        {
            return new Actor.ActorParticipant()
            {
                Id = "test@dragoncon.com",
                FullName = "משתמש מערכת",
                SystemRoles =
                {
                    SystemRoles.ContentManager,
                    SystemRoles.ConventionManager,
                    SystemRoles.ReceptionStaff,
                    SystemRoles.UsersManager
                },
                ConventionRoles =
                {
                    ConventionRoles.Staff,
                    ConventionRoles.GameMaster,
                    ConventionRoles.Volunteer
                }
            };
        }

        private Actor.ActorSystemState LoadSystemState(IDocumentSession _session)
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

                var result = new Actor.ActorSystemState()
                {
                    Configurations = config,

                    AgeGroups = ageGroups,
                    Activities = activities.Where(x => x.IsSubActivity == false).ToList(),

                    ConventionId = convention.Id,
                    ConventionName = convention.Name,
                    Location = convention.Location,
                    TagLine = convention.TagLine,
                    Halls = halls.Where(x => x.Value != null).Select(x => x.Value).ToList(),
                    Days = days.Where(x => x.Value != null).Select(x => x.Value).ToList(),
                    Tickets = tickets.Where(x => x.Value != null).Select(x => x.Value).ToList()
                };
                
                foreach (var activity in activities)
                {
                    result.ObjectIdAndValue[activity.Id] = activity.Name;
                }
                foreach (var day in days)
                {
                    result.ObjectIdAndValue[day.Key] = day.Value.GetDescription();
                }

                foreach (var age in ageGroups)
                {
                    result.ObjectIdAndValue[age.Id] = age.GetDescription();
                }
               
                foreach (var hall in halls)
                {
                    result.ObjectIdAndValue[hall.Key] = hall.Value.Name;
                }

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
