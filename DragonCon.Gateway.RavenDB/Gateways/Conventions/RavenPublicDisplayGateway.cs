using System;
using System.Linq;
using DragonCon.Features.Convention;
using DragonCon.Features.Convention.Home;
using DragonCon.Features.Convention.Landing;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.UserDisplay;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Gateways.Conventions
{
    public class RavenPublicDisplayGateway : RavenGateway, IDisplayPublicGateway
    {
        public RavenPublicDisplayGateway(IServiceProvider provider) : base(provider)
        {
        }

        public AboutViewModel BuildAbout()
        {
            var results = Session.Query<UserEngagement>()
                .Include(x => x.ParticipantId)
                .Where(x => x.Roles.Any(y => y == ConventionRoles.Officer) &&
                            x.ConventionId == Actor.DisplayConvention.ConventionId)
                .ToList();

            return new AboutViewModel
            {
                OfficerLines = results.Select(x =>
                    new OfficerLine
                    {
                        Description = x.RoleDescription,
                        Name = Session.Load<LongTermParticipant>(x.ParticipantId).FullName
                    }).ToList()
            };
        }

        public Answer SendContactUs(ContactUsViewModel viewModel)
        {
            throw new NotImplementedException();
        }
    }
}
