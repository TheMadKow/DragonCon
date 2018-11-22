using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Logical.Gateways.Home;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Logical.Controllers.Management
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
