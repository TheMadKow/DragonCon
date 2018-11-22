﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Profiling;

namespace DragonCon.Features.Shared
{
    public class DragonController<T> : Controller
    where T : IGateway
    {
        protected const int ResultsPerPage = 30;

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
            using (Profiler.Step("OnActionExecuted"))
            {
                base.OnActionExecuted(context);

                if (context.Result is ViewResult result)
                {
                    if (result.Model is IDisplayPaginationViewModel vmPage)
                    {
                        vmPage.Pagination.SetServerActions(
                            FetchRouteData("area"),
                            FetchRouteData("controller"),
                            FetchRouteData("action"));
                    }
                }
            }
        }

        private string FetchRouteData(string type)
        {
            if (ControllerContext.RouteData.Values.ContainsKey(type))
                return ControllerContext.RouteData.Values[type].ToString();
            return string.Empty;
        }
    }
}
