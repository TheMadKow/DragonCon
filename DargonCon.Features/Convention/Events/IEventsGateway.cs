using DragonCon.Features.Convention.Shared;
using DragonCon.Features.Shared;

namespace DragonCon.Features.Convention.Events
{
    public interface IDisplayEventsGateway : IGateway
    {
        DisplayEventsViewModel BuildEvents(IDisplayPagination pagination, DisplayEventsViewModel.Filters filters = null);
    }
}
