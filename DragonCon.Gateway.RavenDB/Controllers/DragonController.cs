using DragonCon.Logical.Gateways;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Raven.Client.Documents.Session;
using StackExchange.Profiling;

namespace DragonCon.Gateway.RavenDB.Controllers
{
    public class DragonController<T> : Controller
    where T : IGateway
    {
        protected T Gateway { get; set; }
        public MiniProfiler Profiler { get; set; }

        public DragonController(T gateway)
        {
            Gateway = gateway;
            Profiler = MiniProfiler.Current;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            using (Profiler.Step("On Action Executing"))
            {
                base.OnActionExecuting(context);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            using (Profiler.Step("On Action Executed"))
            {
                base.OnActionExecuted(context);
            }
        }
    }
}
