using DragonCon.Features.Shared;

namespace DragonCon.Features.Convention.Events
{
    public interface IDisplayEventsGateway : IGateway
    {
        DisplayEventsViewModel BuildEvents();
    }
}
