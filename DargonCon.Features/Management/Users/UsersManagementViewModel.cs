using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DragonCon.Features.Management.Users
{
    public class UsersManagementViewModel : IDisplayPaginationViewModel
    {
        public class Filters {
            public string Role { get; set; }
            public string Payment { get; set; }
        }

        public IDisplayPagination Pagination { get; set; }
        
        public Filters ActiveFilters { get; set; }
        public string ActiveConvention { get;set; } 
        

        public List<FullParticipantWrapper> FullParticipants { get; set; } = new List<FullParticipantWrapper>();
        public List<LimitedParticipantWrapper> LimitedParticipants { get; set; } = new List<LimitedParticipantWrapper>();
    }
}
