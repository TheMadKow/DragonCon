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
    [Authorize(policy: Policies.Types.AtLeastManagementViewer)]
    public class ParticipantsController : DragonController<IManagementParticipantsGateway>
    {
        public ParticipantsController(IServiceProvider service) : base(service)
        {
        }

        #region Display
        [HttpGet]
        public IActionResult Manage(int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage));
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Manage(ParticipantsManagementViewModel.Filters filters, 
                                    int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildIndex(DisplayPagination.BuildForGateway(page, perPage), filters);
            return View(viewModel);

        }

        [HttpPost]
        public IActionResult ManageSearch(string searchWords, int page = 0, int perPage = ResultsPerPage)
        {
            var viewModel = Gateway.BuildSearchIndex(DisplayPagination.BuildForGateway(page, perPage), searchWords);
            return View("Manage", viewModel);

        }

        #endregion

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

        [HttpGet]
        public IActionResult UpdateRoles(string collection, string id)
        {
            var participantId = id.FixRavenId(collection);
            var viewModel = Gateway.GetRolesViewModel(participantId);
            return View("UpdateRoles", viewModel);
        }

        [HttpGet]
        public IActionResult CreateUpdateParticipant(string participantId = null)
        {
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
        ParticipantsManagementViewModel BuildIndex(IDisplayPagination pagination, ParticipantsManagementViewModel.Filters filters = null);
        ParticipantsManagementViewModel BuildSearchIndex(IDisplayPagination pagination, string searchWords = null);
        ParticipantCreateUpdateViewModel GetParticipantViewModel(string participantId);
        UpdateRolesViewModel GetRolesViewModel(string participantId);
        Answer UpdateRoles(string viewmodelParticipantId, string[] sysKeys, string[] conKeys);
        Task<Answer> UpdateParticipant(ParticipantCreateUpdateViewModel viewmodel);
        Task<Answer> CreateParticipant(ParticipantCreateUpdateViewModel viewmodel);
    }
}
