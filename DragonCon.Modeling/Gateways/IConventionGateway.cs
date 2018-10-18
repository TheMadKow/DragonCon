using System.Collections.Generic;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Events;

namespace DragonCon.Modeling.Gateways
{
    public interface IConventionGateway
    {
        ConventionWrapper GetConventionWrapper(string id);
        void StoreConvention(ConventionWrapper convention);
    }
}