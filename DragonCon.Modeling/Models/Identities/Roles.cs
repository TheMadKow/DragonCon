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
        Officer,
        [Description("צוות הנחיה")]
        GameHost,
        [Description("צוות מתנדבים")]
        Volunteer
    }
}
