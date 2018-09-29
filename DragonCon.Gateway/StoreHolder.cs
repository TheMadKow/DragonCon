using System.Linq;
using Raven.Client.Documents;

namespace DragonCon.RavenDB
{
    public class StoreHolder
    {
        public StoreHolder(string dbName, params string[] connectionStrings)
        {
            Store = new DocumentStore
            {
                Urls = connectionStrings.ToArray(),
                Database = dbName
            };
        }

        public IDocumentStore Store { get; private set; }
    }
}
