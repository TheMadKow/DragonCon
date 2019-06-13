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

        public ConDay ConDay { get; set; }
        public EventSystem EventSystem { get; set; }
        public EventActivity EventActivity { get;set; }
        public IParticipant GameMaster { get; set; }
        public IList<IParticipant> Helpers { get; set; }
        public IList<IParticipant> Participants { get; set; }
        public Hall Hall { get;set; }
    }
}
