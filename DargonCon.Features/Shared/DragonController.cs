using System;
using System.Linq;
using DragonCon.Modeling.Models.Identities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace DragonCon.Features.Shared
{
    public class DragonController<T> : Controller
    where T : IGateway
    {
        protected const int ResultsPerPage = 45;

        protected T Gateway { get; set; }
        protected IActor Actor { get; set; }
        protected IServiceProvider Service {get;set;}

        protected void SetActiveTab(string tab)
        {
            ViewBag.HelperTab = tab;
        }

        protected void SetUserError(string title, string description)
        {
            ViewBag.ErrorTitle = title;
            ViewBag.ErrorDescription = description;
        }

        protected string ParseModelErrors()
        {
            var invalidProperty = 
                ModelState.First(x => x.Value.ValidationState == ModelValidationState.Invalid);
            return invalidProperty.Value.Errors.FirstOrDefault()?.ErrorMessage ?? "אנא נסו שוב";
        }

        protected void SetUserSuccess(string title, string description)
        {
            ViewBag.SuccessTitle = title;
            ViewBag.SuccessDescription = description;
        }

        public DragonController(IServiceProvider service)
        {
            Gateway = service.GetRequiredService<T>();
            Service = service;
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
                Actor = Service.GetRequiredService<IActor>();
                base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
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

        private string FetchRouteData(string type)
        {
            if (ControllerContext.RouteData.Values.ContainsKey(type))
                return ControllerContext.RouteData.Values[type].ToString();
            return string.Empty;
        }
    }
}
