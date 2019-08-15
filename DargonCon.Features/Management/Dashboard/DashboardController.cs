using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Identities.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Management.Dashboard
{
    [Area("Management")]
    [Authorize(policy: Policies.Types.AtLeastManagementViewer)]
    public class DashboardController : DragonController<NullGateway>
    {
        public DashboardController(NullGateway gateway) : base(gateway)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
