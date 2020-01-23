using System;
using DragonCon.Features.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Convention.Events
{
    [Area("Convention")]
    public class EventsController : DragonController<IDisplayEventsGateway>
    {
        public EventsController(IServiceProvider service) : 
            base(service)  { }

        public IActionResult Index(int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildEvents();
            return View(viewModel);
        }
    }
}
