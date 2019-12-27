using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace DragonCon.Modeling.Models.Identities.Policy
{
    public static class Policies
    {
        public static class Types
        {
            public const string ManagementAreaViewer = "Management/Viewer";
            public const string ManagementAreaManager = "Management/Manager";

            public const string ReceptionManagement = "Reception/Manager";
            public const string EventsManagement = "Events/Manager";
            public const string UsersManagement = "Users/Manager";
            public const string ConventionManagement = "Convention/Manager";
        }

        public static Dictionary<string, IAuthorizationRequirement> GetPolicies()
        {
            var result = new Dictionary<string, IAuthorizationRequirement>
            {
                {Types.ManagementAreaViewer, new RolesRequirement(SystemRoles.ReceptionManager, SystemRoles.ContentManager, SystemRoles.UsersManager, SystemRoles.ConventionManager)},
                {Types.ManagementAreaManager, new RolesRequirement(SystemRoles.ContentManager, SystemRoles.UsersManager, SystemRoles.ConventionManager)},

                {Types.ReceptionManagement, new RolesRequirement(SystemRoles.ReceptionManager, SystemRoles.ConventionManager)},
                {Types.EventsManagement, new RolesRequirement(SystemRoles.ContentManager, SystemRoles.ConventionManager)},
                {Types.UsersManagement, new RolesRequirement(SystemRoles.UsersManager, SystemRoles.ConventionManager)},
                {Types.ConventionManagement, new RolesRequirement(SystemRoles.ConventionManager)},
            };
            return result;
        }

        public static void AddScopedPolicyHandlers(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, RolesRequirementHandler>();
        }
    }

}
