using System;
using DragonCon.Features.Convention.Home;
using DragonCon.Features.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Convention.Landing
{
    [Area("Convention")]
    public class LandingController : DragonController<IConventionPublicGateway>
    {
        public LandingController(IServiceProvider service) : 
            base(service)  { }

        [HttpGet("/Landing")]
        [HttpGet("/Convention/Landing")]
        [HttpGet("/Convention/Landing/Index")]
        public IActionResult Index()
        {
            var viewModel = Gateway.BuildLanding();
            return View(viewModel);
        }
    }
}
