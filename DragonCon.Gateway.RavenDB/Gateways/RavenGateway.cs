using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using DragonCon.Logical;
using DragonCon.Logical.Communication;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Identities;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Gateways
{
    public abstract class RavenGateway : IDisposable
    {
        public IDocumentSession Session => _session;
        protected IIdentityFacade Identities { get; private set; }
        protected IActor Actor { get; private set; }
        protected ICommunicationHub Hub { get; private set; }
        private readonly StoreHolder _holder;
        private readonly IDocumentSession _session;


        protected RavenGateway(IServiceProvider provider)
        {
            _holder = provider.GetRequiredService<StoreHolder>();
            _session = _holder.Store.OpenSession();

            Actor = provider.GetRequiredService<IActor>();
            Hub = provider.GetRequiredService<ICommunicationHub>();
            Identities = provider.GetRequiredService<IIdentityFacade>();
        }

        protected ConventionRolesContainer LoadConventionRolesContainer(string conventionId = null)
        {
            if (conventionId == null)
                conventionId = Actor.SystemState.ConventionId;

            var result = Session.Query<ConventionRolesContainer>().SingleOrDefault(x => x.ConventionId == conventionId);
            if (result == null)
            {
                result = new ConventionRolesContainer
                {
                    ConventionId = conventionId,
                    UsersAndRoles = new Dictionary<string, List<ConventionRoles>>()
                };
                Session.Store(result);
            }

            return result;
        }

        protected void StoreConventionRolesContainer(ConventionRolesContainer convetion)
        {
            Session.Store(convetion);
        }



        private void ReleaseUnmanagedResources()
        {
            _session?.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RavenGateway()
        {
            Dispose(false);
        }
    }
}
