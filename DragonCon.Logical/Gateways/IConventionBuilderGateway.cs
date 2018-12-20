using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.Conventions;

namespace DragonCon.Logical.Gateways
{
    public interface IConventionBuilderGateway
    {
        ConventionWrapper GetConventionWrapper(string id);
        void StoreConvention(ConventionWrapper convention);
    }
}
