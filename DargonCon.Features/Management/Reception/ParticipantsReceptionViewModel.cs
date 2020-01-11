using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Conventions;

namespace DragonCon.Features.Management.Reception
{
    public class ParticipantsReceptionViewModel
    {
        public IDisplayPagination Pagination { get; set; }
        public List<EngagementWrapper> Participants { get; set; } = new List<EngagementWrapper>();
    }
}
