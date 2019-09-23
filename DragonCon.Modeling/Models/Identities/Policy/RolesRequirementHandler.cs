using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DragonCon.Modeling.Models.Identities.Policy
{
    public class RolesRequirementHandler : AuthorizationHandler<RolesRequirement>
    {
        public RolesRequirementHandler(IActor actor)
        {
            Actor = actor;
        }

        public IActor Actor { get; }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesRequirement requirement)
        {

            if (Actor == null)
            {
                context.Fail();
            }

            foreach (var allowedRole in requirement.AllowedRoles)
            {
                if (Actor.HasSystemRole(allowedRole))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}
