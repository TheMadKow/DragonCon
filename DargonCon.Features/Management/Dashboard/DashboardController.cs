using DragonCon.Features.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Management.Dashboard
{
    [Area("Management")]
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
