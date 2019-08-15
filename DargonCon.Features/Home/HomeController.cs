using System;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Identities;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Home
{
    public class HomeController : DragonController<NullGateway>
    {
        public HomeController(IServiceProvider service) : 
            base(service)  { }

        public IActionResult Index()
        {
            return View();
        }
    }
}
