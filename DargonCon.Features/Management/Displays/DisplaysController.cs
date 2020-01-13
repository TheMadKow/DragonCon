using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DragonCon.Features.Management.Convention;
using DragonCon.Features.Shared;
using DragonCon.Logical.Convention;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Identities.Policy;
using DragonCon.Modeling.Models.System;
using DragonCon.Modeling.Models.UserDisplay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace DragonCon.Features.Management.Displays
{
    [Area("Management")]
    [Authorize(policy: Policies.Types.ConventionManagement)]
    public class DisplaysController : DragonController<IManagementDisplaysGateway>
    {
        public DisplaysController(ConventionBuilder builder, IServiceProvider service) : base(service)
        {
        }

        [HttpGet]
        public IActionResult Manage(string tab = null, string message = null)
        {
            var viewModel = Gateway.BuildDisplays();
            if (tab.IsNotEmptyString())
            {
                SetActiveTab(tab);
            }
            if (message.IsNotEmptyString())
            {
                SetUserError("תקלה", message);
            }
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult RemoveSlide(string slideId)
        {
            var message = string.Empty;
            var answer = Gateway.RemoveSlide(slideId);
            if (answer.AnswerType != AnswerType.Success)
                message = answer.Message;

            return RedirectToAction("Manage", new
            {
                tab = "slides",
                message
            });
        }

        [HttpPost]
        public IActionResult AddSlide(string imageLink, string imageTitle)
        {
            var slide = new DynamicSlideItem
            {
                ImageUrl = imageLink,
                Caption = imageTitle,
                ConventionId = Actor.ManagedConvention.ConventionId
            };

            var message = string.Empty;
            var answer = Gateway.AddSlide(slide);
            if (answer.AnswerType != AnswerType.Success)
                message = answer.Message;
            return RedirectToAction("Manage", new
            {
                tab = "slides",
                message
            });

        }
    }

    public interface IManagementDisplaysGateway : IGateway
    {
        Answer AddSlide(DynamicSlideItem slide);
        Answer RemoveSlide(string slideId);

        DisplaysViewModel BuildDisplays();
    }
}
