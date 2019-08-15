using System.ComponentModel;

namespace DragonCon.Modeling.Models.Identities
{

    public enum SystemRoles
    {
        [Description("ניהול משתמשים")]
        UsersManager,
        [Description("ניהול תוכן")]
        ContentManager,
        [Description("ניהול אירוע")]
        ConventionManager,
        [Description("צוות קבלה")]
        ReceptionStaff
    }


    public enum ConventionRoles
    {
        [Description("צוות מתנדבים")]
        Volunteer,
        [Description("צוות הנחיה")]
        GameMaster
    }
}
