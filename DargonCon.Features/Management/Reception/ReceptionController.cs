using System;
using DragonCon.Features.Management.Dashboard;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Identities.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Management.Reception
{
    [Area("Management")]
    [Authorize(policy: Policies.Types.ReceptionManagement)]
    public class ReceptionController : DragonController<IManagementStatisticsGateway>
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
    }
}
