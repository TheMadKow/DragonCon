using DragonCon.Features.Convention.Home;
using DragonCon.Features.Convention.Landing;
using DragonCon.Features.Shared;

namespace DragonCon.Features.Convention
{
    public interface IConventionPublicGateway : IGateway
    {
        AboutViewModel BuildAbout();
    }
}