using System;
using DragonCon.Modeling.Models.Identities;
using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Gateways
{
    public abstract class RavenGateway : IDisposable
    {
        private readonly StoreHolder _holder;
        protected IActor Actor { get; private set; }

        private readonly IDocumentSession _session;
        public IDocumentSession Session => _session;


        protected RavenGateway(StoreHolder holder, IActor actor)
        {
            _holder = holder;
            Actor = actor;
            _session = _holder.Store.OpenSession();
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
