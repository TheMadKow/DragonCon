using System;
using DragonCon.Features.Convention.Shared;
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
            var viewModel = Gateway.BuildEvents(DisplayPagination.BuildForGateway(page, perPage));
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(DisplayEventsViewModel.Filters ActiveFilters, 
            int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildEvents(DisplayPagination.BuildForGateway(page, perPage), ActiveFilters);
            return View(viewModel);

        }



    }
}
