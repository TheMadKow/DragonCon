using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DragonCon.Modeling.Models.Identities.Policy
{
    public class RolesRequirementHandler : AuthorizationHandler<RolesRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesRequirement requirement)
        {

            if (context.User == null)
            {
                context.Fail();
            }

            foreach (var allowedRole in requirement.AllowedRoles)
            {
                if (context.User.IsInRole(allowedRole.ToString()))
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
