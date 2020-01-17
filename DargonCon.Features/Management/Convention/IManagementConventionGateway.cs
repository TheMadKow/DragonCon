using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.System;

namespace DragonCon.Features.Management.Convention
{
    public interface IManagementConventionGateway : IGateway
    {
        ConventionManagementViewModel BuildConventionList(IDisplayPagination pagination);
        ConventionUpdateViewModel BuildConventionUpdate(string conId);
        Answer SetAsManaged(string id);
        Answer SetAsDisplay(string id);
        Answer UpdateSettings(string conventionId, ConventionSettings settings);
    }
}
