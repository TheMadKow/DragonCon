using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Raven.Client.Documents.Session;
using StackExchange.Profiling;

namespace DragonCon.Gateway.RavenDB.Controllers
{
    public class RavenController : Controller
    {
        public RavenController(AsyncDocumentSession session)
        {
            RavenSessionAsync = session;
            ActionProfiler = MiniProfiler.Current;
        }

        public MiniProfiler ActionProfiler { get; set; }

        public AsyncDocumentSession RavenSessionAsync;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            using (ActionProfiler.Step("On Action Executing"))
            {
                base.OnActionExecuting(context);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            using (ActionProfiler.Step("On Action Executed"))
            {
                base.OnActionExecuted(context);
                RavenSessionAsync.SaveChangesAsync().GetAwaiter().GetResult();
                RavenSessionAsync.Dispose();
            }
        }
    }
}
