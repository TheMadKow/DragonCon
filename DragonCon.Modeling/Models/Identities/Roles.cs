namespace DragonCon.Modeling.Models.Identities
{
    public static class Roles
    {
        public const string UsersManager = "role-users-manager";
        public const string ContentManager = "role-content-manager";
        public const string ConventionManager = "role-convention-manager";

        private const string GameMaster = "role-game-master";
        private const string GameHelper = "role-game-helper";
        private const string Volunteer = "role-volunteer";

        public static string GameMasterForConvention(string conventionId) => $"{GameMaster}/{conventionId}";
        public static string GameHelperForConvention(string conventionId) => $"{GameHelper}/{conventionId}";
        public static string VolunteerForConvention(string conventionId) => $"{Volunteer}/{conventionId}";
    }
}
