using System;
using System.Collections.Generic;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Payment;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Modeling.Models.Identities
{
    public class ShortTermParticipant : IParticipant
    {
        public string Id { get; set;}
        public int YearOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;

        public string FirstName
        {
            get
            {
                try
                {
                    var breakName = FullName.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                    return breakName[0].Trim();
                }
                catch
                {
                    return FullName;
                }
            }
        }

        public string FullName { get; set; } = string.Empty;
        public string CreatedById { get; set; } = string.Empty;
    }
}