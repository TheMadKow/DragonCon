using System;
using System.Threading.Tasks;
using DragonCon.Features.Management.Participants;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;
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
        public IActionResult SearchParticipant(string searchWords, int page, int perPage = 100)
        {
            var viewModel = Gateway.BuildParticipantSearch(DisplayPagination.BuildForGateway(page, perPage), searchWords);
            return View("_PartialAjaxUserDisplay", viewModel);
        }

        [HttpGet]
        public IActionResult AddNewParticipant()
        {
            var viewModel = new ParticipantCreateUpdateViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewParticipant(ParticipantCreateUpdateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var answer = await Gateway.AddParticipant(viewModel);
                if (answer.AnswerType == AnswerType.Success)
                {
                    return RedirectToAction("RegisterEvents",
                        "Reception",
                        new
                        {
                            area = "Management",
                            userId = answer.Message
                        });
                }
                else
                {
                    SetUserError("תקלה במשתמש חדש", answer.Message);
                    return View("AddNewParticipant", viewModel);
                }
            }

            SetUserError("תקלה במידע שהתקבל", ParseModelErrors());
            return View("AddNewParticipant", viewModel);
        }

        [HttpGet]
        public IActionResult RegisterEvents(string userId)
        {
            // TODO Build
            return View();
        }

        // TODO Register as POST

        [HttpGet]
        public IActionResult TicketAndPayment(string userId)
        {
            // TODO Build
            return View();
        }
        // TODO TicketAndPayment as POST


        [HttpGet]
        public IActionResult TicketForPrint(string userId)
        {
            // TODO Build
            return View();
        }

        [HttpGet]
        public IActionResult AvailableEvents()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SearchEvents()
        {
            return View();
        }
    }


    public interface IManagementReceptionGateway : IGateway
    {
        Task<Answer> AddParticipant(ParticipantCreateUpdateViewModel viewModel);
        ParticipantsReceptionViewModel BuildParticipantSearch(IDisplayPagination buildForGateway, string searchWords);
    }
}
