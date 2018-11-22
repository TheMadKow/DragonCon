using DragonCon.Features.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Internal;
using SystemClock = NodaTime.SystemClock;

namespace DragonCon.Features.Management.Convention
{
    [Area("Management")]
    public class ConventionController : DragonController<IConventionGateway>
    {
        public ConventionController(IConventionGateway gateway) : 
            base(gateway)
        {
        }

        [HttpGet]
        public IActionResult Manage(int page = 0, int perPage = ResultsPerPage)
        {
            var instant = SystemClock.Instance.GetCurrentInstant().ToString();
            var conventionViewModel = Gateway.BuildConventionList(DisplayPagination.BuildForGateway(page, perPage));
            return View(conventionViewModel);
        }
    }
}
