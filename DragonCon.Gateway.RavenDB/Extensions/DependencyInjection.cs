using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRavenDbAsyncSession(
            this IServiceCollection services,
            IDocumentStore docStore)
        {
            return services.AddScoped(_ => docStore.OpenAsyncSession());
        }

    }
}
