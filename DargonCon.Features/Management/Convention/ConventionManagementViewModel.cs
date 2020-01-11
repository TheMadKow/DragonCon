using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.System;

namespace DragonCon.Features.Management.Convention
{
    public class ConventionManagementViewModel : IDisplayPaginationViewModel
    {
        public IDisplayPagination Pagination { get; set; }
        public List<ConventionWrapper> Conventions { get; set; } = new List<ConventionWrapper>();
    }
}
