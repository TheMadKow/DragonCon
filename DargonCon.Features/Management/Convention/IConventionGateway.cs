using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Conventions;

namespace DragonCon.Features.Management.Convention
{
    public interface IConventionGateway : IGateway
    {
        ConventionManagementViewModel BuildConventionList(IDisplayPagination pagination);
        


        ConventionWrapper GetConventionWrapper(string id);
        void StoreConvention(ConventionWrapper convention);
    }
}
