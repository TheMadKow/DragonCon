using System.Collections.Generic;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Identities;

namespace DragonCon.Modeling.Models.Events
{
    public class ConEventWrapper : Wrapper<ConEvent>
    {
        public ConEventWrapper() : base()
        {
        }

        public ConEventWrapper(ConEvent model) : base(model){}

        public Day Day { get; set; }
        public EventSystem EventSystem { get; set; }
        public EventActivity EventActivity { get;set; }
        public IList<IParticipant> GameMasters { get; set; }
        public IList<IParticipant> Participants { get; set; }
        public AgeRestriction AgeRestriction { get; set; }
        public Hall Hall { get;set; }
    }
}
