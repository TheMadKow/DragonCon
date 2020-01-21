using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Identities.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Management.Events
{
    [Area("Management")]
    [Authorize(Policies.Types.ContentManagement)]
    public class EventsController : DragonController<IManagementEventsGateway>
    {
        public EventsController(IServiceProvider service) : base(service)
        {
        }

        #region Hosts Selector
        [HttpGet]
        [IgnoreAntiforgeryToken]
        public string GetParticipants(string term)
        {
            if (term.IsEmptyString())
                return string.Empty;

            var participants = Gateway.SearchParticipants(term);

            return new Select2ViewModel
            {
                Results = participants
                    .Where(x => x != null)
                    .Select(x => new Select2Option
                    {
                        Id = x.Id,
                        Text = x.FullName,
                        Brackets = x.Email
                    }).ToList()
            }.AsJson();
        }
        #endregion

        #region Display
        [HttpGet]
        public IActionResult Manage(int page = 0, int perPage = ResultsPerPage, string tab = null)
        {
            ViewBag.HelperTab = tab;
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage));
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Manage(EventsManagementViewModel.Filters ActiveFilters,
                                    int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage), ActiveFilters);
            return View(viewModel);

        }

        [HttpPost]
        public IActionResult ManageSearch(string searchWords, int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage), searchWords);
            return View("Manage", viewModel);

        }
        #endregion

        #region Activity
        [HttpPost]
        public Answer DeleteEventActivity(string activityId)
        {
            var answer = Gateway.DeleteActivity(activityId);
            return answer;
        }

        [HttpGet]
        public IActionResult UpdateEventActivity(string activityId)
        {
            var viewModel = Gateway.GetActivityViewModel(activityId);
            return CreateUpdateEventActivity(viewModel);
        }


        [HttpGet]
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
        public IActionResult CreateUpdateEventActivityPost(ActivityCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Id))
                return CreateEventActivity(viewmodel);
            else
                return UpdateEventActivity(viewmodel);
        }

        private IActionResult UpdateEventActivity(ActivityCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Name))
            {
                SetUserError("תקלה", "שם פעילות ריק");
                return CreateUpdateEventActivity(viewmodel);
            }

            var filteredList = viewmodel.SubActivities.Where(x => x.IsDeleted == false && x.Name.IsNotEmptyString()).ToList();
            var answer = Gateway.UpdateExistingActivity(viewmodel.Id, viewmodel.Name, filteredList);
            if (answer.AnswerType != AnswerType.Success)
            {
                SetUserError("תקלה", answer.Message);
                return CreateUpdateEventActivity(viewmodel);
            }

            return RedirectToAction("Manage", new
            {
                tab = "settings"
            });
        }

        private IActionResult CreateEventActivity(ActivityCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Name))
            {
                SetUserError("תקלה", "שם פעילות ריק");
                return CreateUpdateEventActivity(viewmodel);
            }

            var filteredList = viewmodel.SubActivities.Where(x => x.IsDeleted == false && x.Name.IsNotEmptyString()).Select(x => x.Name).ToList();
            var answer = Gateway.AddNewActivity(viewmodel.Name, filteredList);
            if (answer.AnswerType != AnswerType.Success)
            {
                SetUserError("תקלה", answer.Message);
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
        public IActionResult UpdateEventAgeRestriction(string restrictionId)
        {
            var viewModel = Gateway.GetAgeRestrictionViewModel(restrictionId);
            return CreateUpdateEventAgeRestriction(viewModel);
        }

        [HttpGet]
        public IActionResult CreateUpdateEventAgeRestriction(AgeSystemCreateUpdateViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new AgeSystemCreateUpdateViewModel();
            }

            return View("CreateUpdateEventAgeRestriction", viewModel);
        }

        [HttpPost]
        public IActionResult CreateUpdateEventAgeRestrictionPost(AgeSystemCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Name))
            {
                SetUserError("תקלה", "שם קבוצה ריק");
                return CreateUpdateEventAgeRestriction(viewmodel);
            }

            var answer = Gateway.AddOrUpdateAgeRestriction(viewmodel);
            if (answer.AnswerType != AnswerType.Success)
            {
                SetUserError("תקלה", answer.Message);
                return CreateUpdateEventAgeRestriction(viewmodel);
            }

            return RedirectToAction("Manage", new
            {
                tab = "settings"
            });
        }

        [HttpPost]
        public Answer DeleteAgeRestriction(string restrictionId)
        {
            return Gateway.DeleteAgeRestriction(restrictionId);
        }
        #endregion

        #region Create Update
        [HttpGet]
        public IActionResult CreateUpdateEvent(string eventId = null)
        {
            var viewModel = Gateway.GetEventViewModel(eventId);
            return View("CreateUpdateEvent", viewModel);
        }

        [HttpPost]
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
                SetUserError("תקלה", answer.Message);
                return View("CreateUpdateEvent", viewmodel);
            }
        }

        [HttpPost]
        public IActionResult QuickUpdate(string id, string status, string hall)
        {
            var answer = Gateway.QuickUpdate(id, status, hall);

            if (answer.AnswerType != AnswerType.Success)
                SetUserError("תקלה", answer.Message);

            return RedirectToAction("Manage");
        }
        #endregion

        [HttpGet]
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
        List<LongTermParticipant> SearchParticipants(string query);
    }
}
