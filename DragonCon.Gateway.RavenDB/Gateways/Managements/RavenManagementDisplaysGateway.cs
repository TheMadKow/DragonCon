using System;
using System.Linq;
using DragonCon.Features.Management.Displays;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.UserDisplay;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Managements
{
    public class RavenManagementDisplaysGateway : RavenGateway, IManagementDisplaysGateway
    {
        public RavenManagementDisplaysGateway(IServiceProvider provider)
            : base(provider)
        {
        }

        public Answer AddSlide(DynamicSlideItem slide)
        {
            try
            {
                Session.Store(slide);
                Session.SaveChanges();
                return Answer.Success;
            }
            catch (Exception e)
            {
                return Answer.Error(e.Message);
            }
        }

        public Answer RemoveSlide(string slideId)
        {
            try
            {
                Session.Delete(slideId);
                Session.SaveChanges();
                return Answer.Success;
            }
            catch (Exception e)
            {
                return Answer.Error(e.Message);
            }
        }

        public DisplaysViewModel BuildDisplays()
        {
            var managedCon = Actor.ManagedConvention.ConventionId;
            var lazyUpdates = Session.Query<DynamicUpdateItem>()
                .Where(x => x.ConventionId == managedCon)
                .OrderByDescending(x => x.Timestamp).Lazily();
            var lazySliders = Session.Query<DynamicSlideItem>()
                .Where(x => x.ConventionId == managedCon).Lazily();
            var lazyEnglish = Session.Query<DynamicEnglish>()
                .Where(x => x.ConventionId == managedCon)
                .Lazily();
            var lazyLocation = Session.Query<DynamicLocation>()
                .Where(x => x.ConventionId == managedCon)
                .Lazily();
            var lazyDay = Session.Query<DynamicDays>()
                .Where(x => x.ConventionId == managedCon)
                .Lazily();

            Session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();

            return new DisplaysViewModel
            {
                Updates = lazyUpdates.Value.ToList(),
                Slides = lazySliders.Value.ToList(),
                English = lazyEnglish.Value.FirstOrDefault(),
                Days = lazyDay.Value.FirstOrDefault(),
                Location = lazyLocation.Value.FirstOrDefault()
            };
        }
    }
}