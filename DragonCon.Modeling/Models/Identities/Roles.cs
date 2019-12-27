using System.ComponentModel;

namespace DragonCon.Modeling.Models.Identities
{

    public enum SystemRoles
    {
        [Description("ניהול משתמשים")]
        UsersManager,
        [Description("ניהול אירועים")]
        ContentManager,
        [Description("ניהול כנסים")]
        ConventionManager,
        [Description("ניהול דלפק קבלה")]
        ReceptionManager
    }


    public enum ConventionRoles
    {
        [Description("סגל כנס")]
        Personal,
        [Description("צוות הנחיה")]
        GameMaster,
        [Description("צוות מתנדבים")]
        Volunteer
    }
}
