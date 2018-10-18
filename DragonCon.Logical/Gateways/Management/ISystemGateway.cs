using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.Conventions;

namespace DragonCon.Logical.Gateways.Management
{
    public interface ISystemGateway
    {
        ConventionWrapper GetConventionWrapper(string id);
        void StoreConvention(ConventionWrapper convention);

    }
}
