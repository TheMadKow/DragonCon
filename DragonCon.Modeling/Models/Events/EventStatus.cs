using System.ComponentModel;

namespace DragonCon.Modeling.Models.Events
{
    public enum EventStatus
    {
        [Description("ממתין")]
        Pending,
        [Description("אושר")]
        Approved,
        [Description("נדחה")]
        Declined,
        [Description("בוטל")]
        Canceled,
    }
}