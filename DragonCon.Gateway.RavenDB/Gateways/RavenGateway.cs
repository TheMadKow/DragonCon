using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Gateways
{
    public abstract class RavenGateway
    {
        private readonly StoreHolder _holder;
        protected IDocumentSession Session => _holder.Store.OpenSession();
        protected IAsyncDocumentSession AsyncSession => _holder.Store.OpenAsyncSession();

        protected RavenGateway(StoreHolder holder)
        {
            _holder = holder;
        }
    }
}
