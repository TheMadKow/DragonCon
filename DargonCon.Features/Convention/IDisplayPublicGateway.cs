using DragonCon.Features.Convention.Home;
using DragonCon.Features.Convention.Landing;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;

namespace DragonCon.Features.Convention
{
    public interface IDisplayPublicGateway : IGateway
    {
        AboutViewModel BuildAbout();
        Answer SendContactUs(ContactUsViewModel viewModel);
    }
}