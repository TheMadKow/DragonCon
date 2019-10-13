using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;

namespace DragonCon.Features.Management.Events
{
    public class EventHistoryViewModel
    {
        public List<UserAction> Actions { get; set; } = new List<UserAction>();

        public string GetHebrewName(string field)
        {
            if (FieldsMapper.ContainsKey(field))
                return FieldsMapper[field];

            return field;
        }
        public Dictionary<string, string> FieldsMapper = new Dictionary<string, string>
        {
            {"Event Creation", "יצירת אירוע"},
            {"Name",  "שם אירוע"},
            {"ConventionDayId",  "יום אירוע"},
            {"ActivityId",  "סוג פעילות"},
            {"SubActivityId",  "תת-סוג פעילות"},
            {"GameMasterIds",  "מנחים"},
            {"AgeId",  "קבוצת גיל"},
            {"Status",  "סטטוס"},
            {"TimeSlot",  "התחלה ומשך"},
            {"IsSpecialPrice", "יש מחיר מיוחד?" },
            {"SpecialPrice", "מחיר מיוחד" },
            {"Size", "הגבלת מספר משתתפים" },
            {"Tags", "תגיות" },
            {"HallId", "אולם" },
            {"HallTable", "שולחן" },
            {"Description", "תיאור" },
            {"SpecialRequests", "בקשות מיוחדות" },
        };
    }
}