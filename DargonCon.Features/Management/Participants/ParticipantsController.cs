using System;
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
        #endregion

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
        ParticipantCreateUpdateViewModel GetParticipantViewModel(string participantId);
        Task<Answer> UpdateParticipant(ParticipantCreateUpdateViewModel viewmodel);
        Task<Answer> CreateParticipant(ParticipantCreateUpdateViewModel viewmodel);
    }
}
