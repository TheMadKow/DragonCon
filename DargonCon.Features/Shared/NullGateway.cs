using DragonCon.Modeling.Models.System;

namespace DragonCon.Features.Shared
{
    public class NullGateway : IGateway
    {
        public SystemConfiguration LoadSystemConfiguration()
        {
            return new SystemConfiguration();
        }
    }
}
