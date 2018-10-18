using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Logical.Gateways.Home;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Logical.Controllers.Home
{
    public class HomeController : DragonController<NullGateway>
    {
        public HomeController(NullGateway gateway) : base(gateway)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
