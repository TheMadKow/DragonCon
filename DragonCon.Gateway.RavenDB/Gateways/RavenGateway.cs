using System;
using DragonCon.Logical;
using DragonCon.Logical.Communication;
using DragonCon.Modeling.Helpers;
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
