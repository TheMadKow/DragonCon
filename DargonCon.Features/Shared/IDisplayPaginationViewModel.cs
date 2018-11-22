using System;
using System.Collections.Generic;
using System.Text;

namespace DragonCon.Features.Shared
{
    public interface IDisplayPaginationViewModel
    {
        IDisplayPagination Pagination { get; set; }
    }
}
