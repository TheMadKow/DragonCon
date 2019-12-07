using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.System;

namespace DragonCon.Features.Management.Convention
{
    public interface IManagementConventionGateway : IGateway
    {
        ConventionManagementViewModel BuildConventionList(IDisplayPagination pagination);
        ConventionUpdateViewModel BuildConventionUpdate(string conId);
        void SetAsManaged(string id);
        void SetAsDisplay(string id);
    }
}
