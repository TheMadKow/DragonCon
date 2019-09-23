using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace DragonCon.Modeling.Models.Identities.Policy
{
    public static class Policies
    {
        public static class Types
        {
            public const string AtLeastEventsManager = "Events/Manager";
            public const string AtLeastUsersManager = "Users/Manager";
            public const string AtLeastManagementViewer = "Management/Viewer";
            public const string AtLeastConventionManager = "Convention/Manager";
        }

        public static Dictionary<string, IAuthorizationRequirement> GetPolicies()
        {
            var result = new Dictionary<string, IAuthorizationRequirement>
            {
                {Types.AtLeastEventsManager, new RolesRequirement(SystemRoles.ContentManager, SystemRoles.ConventionManager)},
                {Types.AtLeastUsersManager, new RolesRequirement(SystemRoles.UsersManager, SystemRoles.ConventionManager)},

                {Types.AtLeastManagementViewer, new RolesRequirement(SystemRoles.ReceptionStaff, SystemRoles.ContentManager, SystemRoles.UsersManager, SystemRoles.ConventionManager)},
                {Types.AtLeastConventionManager, new RolesRequirement(SystemRoles.ConventionManager)},
            };
            return result;
        }

        public static void AddScopedPolicyHandlers(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, RolesRequirementHandler>();
        }
    }

}
