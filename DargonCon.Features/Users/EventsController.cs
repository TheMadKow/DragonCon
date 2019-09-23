using System;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Users
{
    [Area("Users")]
    public class EventsController  : DragonController<IEventsGateway>
    {
        public EventsController(IServiceProvider service) : base(service) {}

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SuggestAnEvent()
        {
            return View(new SuggestEventViewModel()
            {
                // TODO add selections
            });
        }

        [HttpPost]
        public IActionResult SuggestAnEvent(SuggestEventViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                Answer ans = Gateway.AddSuggestedEvent(viewmodel);
                if (ans.AnswerType == AnswerType.Error)
                    //TODO implement error shower
                    // TODO Log..
                    return View(viewmodel);
                else
                    return RedirectToAction("Index");
            }
            else
            {
                return View(viewmodel);
            }
        }

    }
}
