using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Identities;

namespace DragonCon.Modeling.Models.Conventions
{
    public class ConventionRolesContainer
    {
        public string Id { get; set; }
        public string ConventionId { get; set; } = string.Empty;
        public Dictionary<string, List<ConventionRoles>> UsersAndRoles { get; set; } = new Dictionary<string, List<ConventionRoles>>();

        public void AddRole(string user, ConventionRoles role)
        {
            if (UsersAndRoles.MissingKey(user))
                UsersAndRoles[user] = new List<ConventionRoles>();

            if (UsersAndRoles[user].Missing(role))
                UsersAndRoles[user].Add(role);
        }

        public void RemoveRole(string user, ConventionRoles role)
        {
            if (UsersAndRoles.MissingKey(user))
                return;

            if (UsersAndRoles[user].Contains(role))
                UsersAndRoles[user].Remove(role);
        }

        public bool HasRole(string user, ConventionRoles role)
        {
            if (UsersAndRoles.MissingKey(user))
                return false;

            return UsersAndRoles[user].Contains(role);
        }

        public List<ConventionRoles> GetRolesForUser(string userId)
        {
            if (UsersAndRoles.MissingKey(userId))
                return new List<ConventionRoles>();

            return UsersAndRoles[userId];
        }
    }
}
