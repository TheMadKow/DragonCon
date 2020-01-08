using System;
using DragonCon.Features.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Convention.Home
{
    [Area("Convention")]
    public class HomeController : DragonController<NullGateway>
    {
        public HomeController(IServiceProvider service) : 
            base(service)  { }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Info()
        {
            return View();
        }

        public IActionResult Volunteering()
        {
            return View();
        }
        public IActionResult VolunteeringForm()
        {
            return View();
        }
        public IActionResult English()
        {
            return View();
        }
        public IActionResult Events()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult AboutStorms()
        {
            return View();
        }

        public IActionResult SiteMap()
        {
            return View();
        }

        public IActionResult Usage()
        {
            return View();
        }



        public IActionResult Error(string err)
        {
            return View();
        }

    }
}
