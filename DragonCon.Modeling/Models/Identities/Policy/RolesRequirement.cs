using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonCon.Modeling.Models.Identities.Policy
{
    public class RolesRequirement : IAuthorizationRequirement
    {
        public List<SystemRoles> AllowedRoles { get; }

        public RolesRequirement(params SystemRoles[] roles)
        {
            AllowedRoles = roles.ToList();
        }
    }}
