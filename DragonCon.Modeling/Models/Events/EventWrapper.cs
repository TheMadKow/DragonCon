using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Identities;

namespace DragonCon.Modeling.Models.Events
{
    public class EventWrapper : Wrapper<Event>
    {
        public EventWrapper() : base()
        {
        }

        public EventWrapper(Event inner) : base(inner){}

        public Day Day { get; set; }
        public Activity Activity { get;set; }
        public Activity SubActivity { get; set; }
        public IList<IParticipant> GameMasters { get; set; }
        public IList<IParticipant> Participants { get; set; }
        public AgeGroup AgeGroup { get; set; }
        public Hall Hall { get;set; }
    }
}
