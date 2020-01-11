using System.Collections.Generic;
using DragonCon.Features.Management.Dashboard;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Events;

namespace DragonCon.Features.Management
{
    public interface IManagementStatisticsGateway : IGateway
    {
        ConventionStatisticsViewModel BuildStatisticView(string conventionId);
    }
}
