using System;
using DragonCon.Features.Convention.Home;
using DragonCon.Features.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Convention.Landing
{
    [Area("Convention")]
    public class LandingController : DragonController<IDisplayPublicGateway>
    {
        public LandingController(IServiceProvider service) : 
            base(service)  { }

        [HttpGet("/Landing")]
        [HttpGet("/Convention/Landing")]
        [HttpGet("/Convention/Landing/Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
