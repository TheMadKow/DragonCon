﻿using System.Collections.Generic;
using System.Linq;
using DragonCon.Features.Management.Events;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Management.Dashboard
{
    [Area("Management")]
    public class EventsController : DragonController<IManagementEventsGateway>
    {
        public EventsController(IManagementEventsGateway gateway) : base(gateway)
        {
        }

        [HttpGet]
        public IActionResult Manage(int page = 0, int perPage = ResultsPerPage, string tab = null)
        {
            ViewBag.HelperTab = tab;
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage));
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Manage(EventsManagementViewModel.Filters filters, 
                                    int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage), 
                                               filters);
            return View(viewModel);

        }

        [HttpGet]
        public IActionResult CreateUpdateEventActivity(ActivitySystemCreateUpdateViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new ActivitySystemCreateUpdateViewModel();
            }

            if (viewModel.Systems == null)
            {
                viewModel.Systems = new List<SystemViewModel>()
                {
                    new SystemViewModel {Name = string.Empty}
                };
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateUpdateEventActivityPost(ActivitySystemCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Id))
                return CreateEventActivity(viewmodel);
            else 
                return UpdateEventActivity(viewmodel);
        }

        private IActionResult UpdateEventActivity(ActivitySystemCreateUpdateViewModel viewmodel)
        {
            throw new System.NotImplementedException();
        }

        private IActionResult CreateEventActivity(ActivitySystemCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Name))
            {
                viewmodel.ErrorMessage = "שם פעילות ריק";
                return CreateUpdateEventActivity(viewmodel);
            }

            var filteredList = viewmodel.Systems.Where(x => x.IsDeleted == false && x.Name.IsNotEmptyString()).Select(x => x.Name).ToList();
            var answer = Gateway.AddNewActivity(viewmodel.Name, filteredList);
            if (answer.AnswerType != AnswerType.Success)
            {
                viewmodel.ErrorMessage = answer.Message;
                return CreateUpdateEventActivity(viewmodel);
            }

            return RedirectToAction("Manage", new
            {
                tab = "activities"
            });
        }
    }

    public interface IManagementEventsGateway : IGateway
    {
        Answer AddNewActivity(string name, List<string> systems);
        EventsManagementViewModel BuildIndex(IDisplayPagination pagination, 
                                             EventsManagementViewModel.Filters filters = null);
    }
}
