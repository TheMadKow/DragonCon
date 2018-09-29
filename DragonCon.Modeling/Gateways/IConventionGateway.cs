using DragonCon.Modeling.Models.Conventions;

namespace DragonCon.Modeling.Gateways
{
    public interface IConventionGateway
    {
        ConventionWrapper GetConventionWrapper(string id);
        void StoreConvention(ConventionWrapper convention);
    }
}