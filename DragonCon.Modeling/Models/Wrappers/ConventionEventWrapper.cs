using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Convention;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.Identities;

namespace DragonCon.Modeling.Models.Wrappers
{
    public class ConventionEventWrapper : ConventionEvent
    {
        public ConventionEventWrapper()
        {
         
        }

        public ConventionDay ConventionDay { get; set; }
        public EventSystem EventSystem { get; set; }
        public EventActivity EventActivity { get;set; }
        public Participant Manager { get; set; }
        public IList<IParticipant> Helpers { get; set; }
        public IList<IParticipant> Participants { get; set; }
        public Hall Hall { get;set; }
    }
}
