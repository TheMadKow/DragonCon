using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Features.Management.Events;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Management.Participants
{
    [Area("Management")]
    [Authorize(Policies.Types.UsersManagement)]
    public class ParticipantsController : DragonController<IManagementParticipantsGateway>
    {
        public ParticipantsController(IServiceProvider service) : base(service)
        {
        }

        #region Display
        [HttpGet]
        public IActionResult Manage(int page = 0, bool allowHistory = false, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage), allowHistory);
            viewModel.AllowHistoryParticipants = allowHistory;
            return View(viewModel);
        }


        [Authorize(Policies.Types.EventsManagement)]
        [HttpPost]
        public async Task<Answer> ResetPassword(string id)
        {
            var answer = await Gateway.ResetPassword(id);
            return answer;
        }


        [HttpPost]
        public IActionResult Manage(ParticipantsManagementViewModel.Filters filters, 
                                    bool allowHistory = false, int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage), allowHistory, filters);
            viewModel.AllowHistoryParticipants = allowHistory;
            return View(viewModel);

        }

        [HttpPost]
        public IActionResult ManageSearch(string searchWords, bool allowHistory = false, int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildSearchIndex(DisplayPagination.BuildForGateway(page, perPage), allowHistory, searchWords);
            viewModel.AllowHistoryParticipants = allowHistory;
            return View("Manage", viewModel);

        }

        #endregion

        [Authorize(Policies.Types.EventsManagement)]
        [HttpPost]
        public IActionResult UpdateRoles(UpdateRolesViewModel viewmodel)
        {
            var conKeys = Request.Form.Keys.Where(x => x.StartsWith("con_")).Select(x => x.Replace("con_", "")).ToArray();
            var sysKeys = Request.Form.Keys.Where(x => x.StartsWith("sys_")).Select(x => x.Replace("sys_", "")).ToArray();
            var answer = Gateway.UpdateRoles(viewmodel.ParticipantId, sysKeys, conKeys);
            if (answer.AnswerType != AnswerType.Success)
            {
                // TODO Add Error Message
                return View("CreateUpdateParticipant", viewmodel);
            }

            return RedirectToAction("Manage");
        }

        [Authorize(Policies.Types.EventsManagement)]
        [HttpGet]
        public IActionResult UpdateRoles(string collection, string id)
        {
            var participantId = id.FixRavenId(collection);
            var viewModel = Gateway.GetRolesViewModel(participantId);
            return View("UpdateRoles", viewModel);
        }

        [HttpGet]
        public IActionResult CreateUpdateParticipant(string collection = null, string id = null)
        {
            var participantId = string.Empty;
            if (collection != null && id != null)
            {
                participantId = id.FixRavenId(collection);
            }

            var viewModel = Gateway.GetParticipantViewModel(participantId);
            return View("CreateUpdateParticipant", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUpdateParticipant(ParticipantCreateUpdateViewModel viewmodel)
        {
            var answer = Answer.Error();
            if (viewmodel.Id.IsNotEmptyString())
                answer = await Gateway.UpdateParticipant(viewmodel);
            else
                answer = await Gateway.CreateParticipant(viewmodel);

            if (answer.AnswerType == AnswerType.Success)
                return RedirectToAction("Manage");
            else
            {
                return View("CreateUpdateParticipant", viewmodel);
            }
        }
    }

    public interface IManagementParticipantsGateway : IGateway
    {
        ParticipantsManagementViewModel BuildIndex(IDisplayPagination pagination, bool allowHistory = false, ParticipantsManagementViewModel.Filters filters = null);
        ParticipantsManagementViewModel BuildSearchIndex(IDisplayPagination pagination, bool allowHistory = false, string searchWords = null);
        ParticipantCreateUpdateViewModel GetParticipantViewModel(string participantId);
        UpdateRolesViewModel GetRolesViewModel(string participantId);
        Answer UpdateRoles(string viewmodelParticipantId, string[] sysKeys, string[] conKeys);
        Task<Answer> UpdateParticipant(ParticipantCreateUpdateViewModel viewmodel);
        Task<Answer> CreateParticipant(ParticipantCreateUpdateViewModel viewmodel);
        Task<Answer> ResetPassword(string id);
    }
}
