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
    public class UsersController : DragonController<IManagementEventsGateway>
    {
        public UsersController(IServiceProvider service) : base(service)
        {
        }

        #region Display
        [HttpGet]
        public IActionResult Manage(int page = 0, int perPage = ResultsPerPage, string tab = null)
        {
            //ViewBag.HelperTab = tab;
            //var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage));
            //return View(viewModel);
            return null;
        }

        [HttpPost]
        public IActionResult Manage(EventsManagementViewModel.Filters filters, 
                                    int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage), 
                                               filters);
            return View(viewModel);

        }
        #endregion

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
                return View("CreateUpdateEvent", viewmodel);
            }
        }
    }

    public interface IManagementUsersGateway : IGateway
    {
        EventsManagementViewModel BuildIndex(IDisplayPagination pagination, EventsManagementViewModel.Filters filters = null);


        Answer AddNewActivity(string name, List<string> systems);
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
