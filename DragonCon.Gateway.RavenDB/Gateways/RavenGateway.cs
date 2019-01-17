using DragonCon.Modeling.Models.System;
using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Gateways
{
    public abstract class RavenGateway
    {
        private readonly StoreHolder _holder;
        protected IDocumentSession Session => _holder.Store.OpenSession();
        protected IAsyncDocumentSession AsyncSession => _holder.Store.OpenAsyncSession();

        public SystemConfiguration LoadSystemConfiguration()
        {
            using (var session = _holder.Store.OpenSession())
            {
                return session.Load<SystemConfiguration>(SystemConfiguration.Id) ?? new SystemConfiguration();
            }
        }

        protected RavenGateway()
        {

        }

        protected RavenGateway(StoreHolder holder)
        {
            _holder = holder;
        }
    }
}
