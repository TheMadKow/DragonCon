using DragonCon.Features.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Home
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
