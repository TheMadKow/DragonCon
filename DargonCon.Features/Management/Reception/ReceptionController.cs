using System;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Identities.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Management.Reception
{
    [Area("Management")]
    [Authorize(policy: Policies.Types.ReceptionManagement)]
    public class ReceptionController : DragonController<IManagementReceptionGateway>
    {
        public ReceptionController(IServiceProvider service) : base(service)
        {
        }

        [HttpGet]
        public IActionResult Manage()
        {
            return View();
        }

        [HttpGet]
        public string GetEventList()
        {
            return string.Empty;
        }

        [HttpGet]
        public string GetParticipantList()
        {
            return string.Empty;
        }

        [HttpPost]
        public IActionResult SearchParticipant(string searchWords, int page, int perPage = 100)
        {
            var viewModel = Gateway.BuildParticipantSearch(DisplayPagination.BuildForGateway(page, perPage), searchWords);
            return View("_PartialAjaxUserDisplay", viewModel);
        }
    }


    public interface IManagementReceptionGateway : IGateway
    {
        ParticipantsReceptionViewModel BuildParticipantSearch(IDisplayPagination buildForGateway, string searchWords);
    }
}
