using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents;

namespace DragonCon.RavenDB
{
    public class StoreHolder
    {
        public StoreHolder(string dbName, string certPath, params string[] connectionStrings)
        {
            var cert = new X509Certificate2(certPath);
                Store = new DocumentStore
            {
                Urls = connectionStrings,
                Certificate = cert,
                Database = dbName
            };
        }

        public IDocumentStore Store { get; private set; }
    }
}
