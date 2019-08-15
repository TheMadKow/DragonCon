using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Modeling.Models.Identities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DragonCon.App.Helpers
{
    public class InitializeActorMiddleware
    {
        private readonly RequestDelegate _next;

        public InitializeActorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, IActor actor)
        {
            actor.Participant = new Actor.ParticipantActor()
            {
                Id = "Test",
                RoleList =
                {
                    SystemRoles.ContentManager, SystemRoles.ConventionManager, SystemRoles.ReceptionStaff,
                    SystemRoles.UsersManager
                },
                FullName = "Master Key"
            };
            actor.Convention = new Actor.ConventionActor()
            {
                Id = "Test",
                Name = "Test"
            };

            return _next(httpContext);
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
