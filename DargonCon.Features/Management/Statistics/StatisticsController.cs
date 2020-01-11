using System;
using DragonCon.Features.Management.Dashboard;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Identities.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Management.Statistics
{
    [Area("Management")]
    [Authorize(policy: Policies.Types.ManagementAreaManager)]
    public class StatisticsController : DragonController<IManagementStatisticsGateway>
    {
        public StatisticsController(IServiceProvider service) : base(service)
        {
        }

        [HttpGet]
        public IActionResult Index(string conventionId)
        {
            var viewModel = new ConventionStatisticsViewModel();
            if (conventionId.IsEmptyString() && Actor.HasManagedConvention)
            {
                conventionId = Actor.ManagedConvention.ConventionId;
            }

            if (conventionId.IsEmptyString())
                return View(viewModel);

            viewModel = Gateway.BuildStatisticView(conventionId);
            return View(viewModel);
        }
    }
}
