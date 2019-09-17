using System;
using DragonCon.Features.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Home.Convention
{
    [Area("Home")]
    public class ConventionController : DragonController<NullGateway>
    {
        public ConventionController(IServiceProvider service) : 
            base(service)  { }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error(string err)
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Usage()
        {
            return View();
        }

    }
}
