using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.System;

namespace DragonCon.Features.Management.Convention
{
    public interface IManagementConventionGateway : IGateway
    {
        ConventionManagementViewModel BuildConventionList(IDisplayPagination pagination);
        void SaveSystemConfiguration(SystemConfiguration config);
        ConventionUpdateViewModel BuildConventionUpdate(string conId);
    }
}
