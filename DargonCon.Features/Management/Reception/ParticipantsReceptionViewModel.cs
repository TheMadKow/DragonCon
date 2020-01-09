using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Identities;

namespace DragonCon.Features.Management.Reception
{
    public class ParticipantsReceptionViewModel
    {
        public IDisplayPagination Pagination { get; set; }
        public List<ParticipantWrapper> Participants { get; set; } = new List<ParticipantWrapper>();
    }
}
