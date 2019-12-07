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
    [Authorize(policy: Policies.Types.EventsManagement)]
    public class EventsController : DragonController<IManagementEventsGateway>
    {
        public EventsController(IServiceProvider service) : base(service)
        {
        }

        #region Display
        [HttpGet]
        [Authorize(policy: Policies.Types.EventsManagement)]
        public IActionResult Manage(int page = 0, int perPage = ResultsPerPage, string tab = null)
        {
            ViewBag.HelperTab = tab;
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage));
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(policy: Policies.Types.EventsManagement)]
        public IActionResult Manage(EventsManagementViewModel.Filters ActiveFilters, 
                                    int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage), ActiveFilters);
            return View(viewModel);

        }

        [HttpPost]
        [Authorize(policy: Policies.Types.EventsManagement)]
        public IActionResult ManageSearch(string searchWords, int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage), searchWords);
            return View("Manage", viewModel);

        }
        #endregion

        #region Activity
        [HttpPost]
        [Authorize(policy: Policies.Types.EventsManagement)]
        public Answer DeleteEventActivity(string activityId)
        {
            var answer = Gateway.DeleteActivity(activityId);
            return answer;
        }

        [HttpGet]
        [Authorize(policy: Policies.Types.EventsManagement)]
        public IActionResult UpdateEventActivity(string activityId)
        {
            var viewModel = Gateway.GetActivityViewModel(activityId);
            return CreateUpdateEventActivity(viewModel);
        }


        [HttpGet]
        [Authorize(policy: Policies.Types.EventsManagement)]
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
        [Authorize(policy: Policies.Types.EventsManagement)]
        public IActionResult CreateUpdateEventActivityPost(ActivityCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Id))
                return CreateEventActivity(viewmodel);
            else 
                return UpdateEventActivity(viewmodel);
        }

        [Authorize(policy: Policies.Types.EventsManagement)]
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

        [Authorize(policy: Policies.Types.EventsManagement)]
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
        [Authorize(policy: Policies.Types.EventsManagement)]
        public IActionResult UpdateEventAgeRestriction(string restrictionId)
        {
            var viewModel = Gateway.GetAgeRestrictionViewModel(restrictionId);
            return CreateUpdateEventAgeRestriction(viewModel);
        }

        [HttpGet]
        [Authorize(policy: Policies.Types.EventsManagement)]
        public IActionResult CreateUpdateEventAgeRestriction(AgeSystemCreateUpdateViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new AgeSystemCreateUpdateViewModel();
            }

            return View("CreateUpdateEventAgeRestriction", viewModel);
        }

        [HttpPost]
        [Authorize(policy: Policies.Types.EventsManagement)]
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
        [Authorize(policy: Policies.Types.EventsManagement)]
        public Answer DeleteAgeRestriction(string restrictionId)
        {
            return Gateway.DeleteAgeRestriction(restrictionId);
        }
        #endregion

        [HttpGet]
        [Authorize(policy: Policies.Types.EventsManagement)]
        public IActionResult CreateUpdateEvent(string eventId = null)
        {
            var viewModel = Gateway.GetEventViewModel(eventId);
            return View("CreateUpdateEvent", viewModel);
        }

        [HttpPost]
        [Authorize(policy: Policies.Types.EventsManagement)]
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

        [HttpPost]
        [Authorize(policy: Policies.Types.EventsManagement)]
        public IActionResult QuickUpdate(string id, string status, string hall)
        {
            var answer = Gateway.QuickUpdate(id, status, hall);

            if (answer.AnswerType != AnswerType.Success)
            {
                // TODO Popup on failure.
            }

            return RedirectToAction("Manage");
        }


        [HttpGet]
        [Authorize(policy: Policies.Types.EventsManagement)]
        public IActionResult ViewEventHistory(string eventId)
        {
            var vm = Gateway.CreateEventHistory(eventId);
            return View(vm);
        }

    }

    public interface IManagementEventsGateway : IGateway
    {
        EventsManagementViewModel BuildIndex(IDisplayPagination pagination, EventsManagementViewModel.Filters filters = null);
        EventsManagementViewModel BuildIndex(IDisplayPagination displayPagination, string searchWords);

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
        Answer QuickUpdate(string id, string status, string hall);

        EventHistoryViewModel CreateEventHistory(string eventId);
    }
}
