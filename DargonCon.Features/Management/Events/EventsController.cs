using DragonCon.Features.Management.Events;
using DragonCon.Features.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Management.Dashboard
{
    [Area("Management")]
    public class EventsController : DragonController<IManagementEventsGateway>
    {
        public EventsController(IManagementEventsGateway gateway) : base(gateway)
        {
        }

        [HttpGet]
        public IActionResult Manage(int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage));
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Manage(EventsManagementViewModel.Filters filters, 
                                    int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage), 
                                               filters);
            return View(viewModel);

        }
    }

    public interface IManagementEventsGateway : IGateway
    {
        EventsManagementViewModel BuildIndex(IDisplayPagination pagination, 
                                             EventsManagementViewModel.Filters filters = null);
    }
}
