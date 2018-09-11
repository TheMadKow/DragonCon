using DragonCon.Modeling.Models.Wrappers;

namespace DragonCon.Modeling.Gateways
{
    public interface IConventionGateway
    {
        ConventionWrapper GetConventionWrapper(string id);
        void StoreConvention(ConventionWrapper convention);
    }
}