using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Features.Management.Events;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Events;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Identities.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Management.Dashboard
{
    [Area("Management")]
    [Authorize(policy: Policies.Types.AtLeastManagementViewer)]
    public class EventsController : DragonController<IManagementEventsGateway>
    {
        public EventsController(IServiceProvider service) : base(service)
        {
        }

        #region Display
        [HttpGet]
        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        public IActionResult Manage(int page = 0, int perPage = ResultsPerPage, string tab = null)
        {
            ViewBag.HelperTab = tab;
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage));
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        public IActionResult Manage(EventsManagementViewModel.Filters filters, 
                                    int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage), 
                                               filters);
            return View(viewModel);

        }
        #endregion

        #region Activity
        [HttpPost]
        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        public Answer DeleteEventActivity(string activityId)
        {
            var answer = Gateway.DeleteActivity(activityId);
            return answer;
        }

        [HttpGet]
        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        public IActionResult UpdateEventActivity(string activityId)
        {
            var viewModel = Gateway.GetActivityViewModel(activityId);
            return CreateUpdateEventActivity(viewModel);
        }


        [HttpGet]
        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        public IActionResult CreateUpdateEventActivity(ActivityCreateUpdateViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new ActivityCreateUpdateViewModel();
            }

            if (viewModel.SubActivities == null)
            {
                viewModel.SubActivities = new List<SubActivityViewModel>()
                {
                    new SubActivityViewModel {Name = string.Empty}
                };
            }

            return View("CreateUpdateEventActivity", viewModel);
        }

        [HttpPost]
        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        public IActionResult CreateUpdateEventActivityPost(ActivityCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Id))
                return CreateEventActivity(viewmodel);
            else 
                return UpdateEventActivity(viewmodel);
        }

        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        private IActionResult UpdateEventActivity(ActivityCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Name))
            {
                viewmodel.ErrorMessage = "שם פעילות ריק";
                return CreateUpdateEventActivity(viewmodel);
            }

            var filteredList = viewmodel.SubActivities.Where(x => x.IsDeleted == false && x.Name.IsNotEmptyString()).ToList();
            var answer = Gateway.UpdateExistingActivity(viewmodel.Id, viewmodel.Name, filteredList);
            if (answer.AnswerType != AnswerType.Success)
            {
                viewmodel.ErrorMessage = answer.Message;
                return CreateUpdateEventActivity(viewmodel);
            }

            return RedirectToAction("Manage", new
            {
                tab = "settings"
            });
        }

        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        private IActionResult CreateEventActivity(ActivityCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Name))
            {
                viewmodel.ErrorMessage = "שם פעילות ריק";
                return CreateUpdateEventActivity(viewmodel);
            }

            var filteredList = viewmodel.SubActivities.Where(x => x.IsDeleted == false && x.Name.IsNotEmptyString()).Select(x => x.Name).ToList();
            var answer = Gateway.AddNewActivity(viewmodel.Name, filteredList);
            if (answer.AnswerType != AnswerType.Success)
            {
                viewmodel.ErrorMessage = answer.Message;
                return CreateUpdateEventActivity(viewmodel);
            }

            return RedirectToAction("Manage", new
            {
                tab = "settings"
            });
        }
        #endregion

        #region Age Restriction
        [HttpGet]
        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        public IActionResult UpdateEventAgeRestriction(string restrictionId)
        {
            var viewModel = Gateway.GetAgeRestrictionViewModel(restrictionId);
            return CreateUpdateEventAgeRestriction(viewModel);
        }

        [HttpGet]
        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        public IActionResult CreateUpdateEventAgeRestriction(AgeSystemCreateUpdateViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new AgeSystemCreateUpdateViewModel();
            }

            return View("CreateUpdateEventAgeRestriction", viewModel);
        }

        [HttpPost]
        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        public IActionResult CreateUpdateEventAgeRestrictionPost(AgeSystemCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Name))
            {
                viewmodel.ErrorMessage = "שם קבוצת גיל ריק";
                return CreateUpdateEventAgeRestriction(viewmodel);
            }

            var answer = Gateway.AddOrUpdateAgeRestriction(viewmodel);
            if (answer.AnswerType != AnswerType.Success)
            {
                viewmodel.ErrorMessage = answer.Message;
                return CreateUpdateEventAgeRestriction(viewmodel);
            }

            return RedirectToAction("Manage", new
            {
                tab = "settings"
            });
        }

        [HttpPost]
        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        public Answer DeleteAgeRestriction(string restrictionId)
        {
            return Gateway.DeleteAgeRestriction(restrictionId);
        }
        #endregion

        [HttpGet]
        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        public IActionResult CreateUpdateEvent(string eventId = null)
        {
            var viewModel = Gateway.GetEventViewModel(eventId);
            return View("CreateUpdateEvent", viewModel);
        }

        [HttpPost]
        [Authorize(policy: Policies.Types.AtLeastEventsManager)]
        public IActionResult CreateUpdateEvent(EventCreateUpdateViewModel viewmodel)
        {
            var answer = Answer.Error();
            if (viewmodel.Id.IsNotEmptyString())
                answer = Gateway.UpdateEvent(viewmodel);
            else
                answer = Gateway.CreateEvent(viewmodel);

            if (answer.AnswerType == AnswerType.Success)
                return RedirectToAction("Manage");
            else
            {
                return View("CreateUpdateEvent", viewmodel);
            }
        }
    }

    public interface IManagementEventsGateway : IGateway
    {
        EventsManagementViewModel BuildIndex(IDisplayPagination pagination, EventsManagementViewModel.Filters filters = null);


        Answer AddNewActivity(string name, List<string> subActivities);
        Answer UpdateExistingActivity(string viewmodelId, string viewmodelName, List<SubActivityViewModel> filteredList);
        Answer DeleteActivity(string activityId);
        ActivityCreateUpdateViewModel GetActivityViewModel(string activityId);
        

        Answer AddOrUpdateAgeRestriction(AgeSystemCreateUpdateViewModel viewmodel);
        AgeSystemCreateUpdateViewModel GetAgeRestrictionViewModel(string restrictionId);
        Answer DeleteAgeRestriction(string restrictionId);

        EventCreateUpdateViewModel GetEventViewModel(string eventId);
        Answer UpdateEvent(EventCreateUpdateViewModel viewmodel);
        Answer CreateEvent(EventCreateUpdateViewModel viewmodel);
    }
}
