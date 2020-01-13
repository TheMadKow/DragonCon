using System;
using System.Linq;
using DragonCon.Features.Convention;
using DragonCon.Features.Convention.Home;
using DragonCon.Features.Convention.Landing;
using DragonCon.Modeling.Models.UserDisplay;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Conventions
{
    public class RavenPublicConventionGateway : RavenGateway, IConventionPublicGateway
    {
        public HomeViewModel BuildHome()
        {
            var lazySliders = Session.Query<DynamicSlideItem>()
                .Where(x => x.ConventionId == Actor.DisplayConvention.ConventionId)
                .Lazily();
            var lazyUpdates = LinqExtensions.OrderByDescending(Session.Query<DynamicUpdateItem>()
                    .Where(x => x.ConventionId == Actor.DisplayConvention.ConventionId), x => x.Date)
                .Take(3)
                .Lazily();

            Session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();
            
            return new HomeViewModel
            {
                Slides = lazySliders.Value.ToList(),
                Updates = lazyUpdates.Value.ToList()
            };
        }

        public LandingViewModel BuildLanding()
        {
            var lazySliders = Session.Query<DynamicSlideItem>()
                .Where(x => x.ConventionId == Actor.DisplayConvention.ConventionId)
                .Lazily();
      
            Session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();

            return new LandingViewModel
            {
                Slides = lazySliders.Value.ToList(),
            };
        }

        public RavenPublicConventionGateway(IServiceProvider provider) : base(provider)
        {
        }
    }
}
