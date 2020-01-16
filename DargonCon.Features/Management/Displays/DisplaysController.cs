using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DragonCon.Features.Management.Convention;
using DragonCon.Features.Shared;
using DragonCon.Logical.Convention;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Identities.Policy;
using DragonCon.Modeling.Models.System;
using DragonCon.Modeling.Models.UserDisplay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace DragonCon.Features.Management.Displays
{
    [Area("Management")]
    [Authorize(policy: Policies.Types.ConventionManagement)]
    public class DisplaysController : DragonController<IManagementDisplaysGateway>
    {
        public DisplaysController(ConventionBuilder builder, IServiceProvider service) : base(service)
        {
        }

        [HttpGet]
        public IActionResult Manage(string tab = null, string message = null)
        {
            var viewModel = Gateway.BuildDisplays();
            if (tab.IsNotEmptyString())
            {
                SetActiveTab(tab);
            }
            if (message.IsNotEmptyString())
            {
                SetUserError("תקלה", message);
            }
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult RemoveSlide(string slideId)
        {
            var message = string.Empty;
            var answer = Gateway.RemoveDisplayItem(slideId);
            if (answer.AnswerType != AnswerType.Success)
                message = answer.Message;

            return RedirectToAction("Manage", new
            {
                tab = "slides",
                message
            });
        }

        [HttpPost]
        public IActionResult AddSlide(string imageLink, string imageTitle)
        {
            var slide = new DynamicSlideItem
            {
                ImageUrl = imageLink,
                Caption = imageTitle,
                ConventionId = Actor.ManagedConvention.ConventionId
            };

            var message = string.Empty;
            var answer = Gateway.AddDisplayItem(slide);
            if (answer.AnswerType != AnswerType.Success)
                message = answer.Message;
            return RedirectToAction("Manage", new
            {
                tab = "slides",
                message
            });

        }

        [HttpPost]
        public IActionResult RemoveUpdate(string updateId)
        {
            var message = string.Empty;
            var answer = Gateway.RemoveDisplayItem(updateId);
            if (answer.AnswerType != AnswerType.Success)
                message = answer.Message;

            return RedirectToAction("Manage", new
            {
                tab = "updates",
                message
            });
        }

        [HttpPost]
        public IActionResult AddUpdate(string updateDate, string updateTitle, string updateLink, string updateDesc)
        {
            var now = DateTime.Now.Date;
            var asLocal = new LocalDate(now.Year, now.Month, now.Day);
            if (updateDate.IsNotEmptyString())
            {
                try
                {
                    var splits = updateDate.Split("-").Select(x => int.Parse(x)).ToArray();
                    asLocal = new LocalDate(splits[0], splits[1], splits[2]);
                }
                catch
                {
                    // Do Nothing
                }
            }

            var slide = new DynamicUpdateItem
            {
                Date = asLocal,
                Title = updateTitle ?? string.Empty,
                Description = updateDesc ?? string.Empty,
                Link = updateLink ?? string.Empty,
                ConventionId = Actor.ManagedConvention.ConventionId
            };

            var message = string.Empty;
            var answer = Gateway.AddDisplayItem(slide);
            if (answer.AnswerType != AnswerType.Success)
                message = answer.Message;
            return RedirectToAction("Manage", new
            {
                tab = "updates",
                message
            });
        }

        [HttpPost]
        public IActionResult AddSponsor(string sponsorName, string sponsorImage, string sponsorUrl)
        {
            var slide = new DynamicSponsorItem
            {
                ImageUrl = sponsorImage,
                SponsorUrl = sponsorUrl,
                Caption = sponsorName,
                ConventionId = Actor.ManagedConvention.ConventionId
            };

            var message = string.Empty;
            var answer = Gateway.AddDisplayItem(slide);
            if (answer.AnswerType != AnswerType.Success)
                message = answer.Message;
            return RedirectToAction("Manage", new
            {
                tab = "sponsors",
                message
            });
        }

        [HttpPost]
        public IActionResult RemoveSponsor(string sponsorId)
        {
            var message = string.Empty;
            var answer = Gateway.RemoveDisplayItem(sponsorId);
            if (answer.AnswerType != AnswerType.Success)
                message = answer.Message;

            return RedirectToAction("Manage", new
            {
                tab = "sponsors",
                message
            });
        }

        [HttpPost]
        public IActionResult SetEnglish(
            string englishId,
            string engLocName,
            string engLocDesc,
            string engLocLink)
        {
            var slide = new DynamicEnglish
            {
                Id = englishId,
                Location = engLocName,
                LocationDescription = engLocDesc,
                LocationMap = engLocLink,
                ConventionId = Actor.ManagedConvention.ConventionId
            };

            var message = string.Empty;
            var answer = Gateway.SetDisplayItem(slide);
            if (answer.AnswerType != AnswerType.Success)
                message = answer.Message;
            return RedirectToAction("Manage", new
            {
                tab = "english",
                message
            });
        }

        [HttpPost]
        public IActionResult SetLinkage(
            string linkId,
            string linkProgram,
            string linkProgramImage,
            string linkYad2,
            string linkVolunteer,
            string linkSuggestGuide)
        {
            var days = new DynamicLinkage
            {
                Id = linkId,
                ProgramImage = linkProgramImage,
                ProgramLink = linkProgram,
                SuggestGuidelinesLink = linkSuggestGuide,
                VolunteerLink = linkVolunteer,
                Yad2FormLink = linkYad2,
                ConventionId = Actor.ManagedConvention.ConventionId
            };

            var message = string.Empty;
            var answer = Gateway.SetDisplayItem(days);
            if (answer.AnswerType != AnswerType.Success)
                message = answer.Message;
            return RedirectToAction("Manage", new
            {
                tab = "linkage",
                message
            });
        }

        public IActionResult SetLocation(
        string locId,
        string locName,
        string locDesc,
        string locImage,
        string locWays,
        string locMap)
        {
            var location = new DynamicLocation()
            {
                Id = locId,
                LocationName = locName,
                LocationDescription = locDesc,
                LocationWaysOfArrival = locWays,
                LocationMap = locMap,
                LocationImage = locImage,
                ConventionId = Actor.ManagedConvention.ConventionId
            };

            var message = string.Empty;
            var answer = Gateway.SetDisplayItem(location);
            if (answer.AnswerType != AnswerType.Success)
                message = answer.Message;
            return RedirectToAction("Manage", new
            {
                tab = "location",
                message
            });
        }
    }

    public interface IManagementDisplaysGateway : IGateway
    {
        DisplaysViewModel BuildDisplays();

        Answer SetDisplayItem<T>(T item)
            where T : DynamicDisplayItem;
        Answer AddDisplayItem(DynamicDisplayItem slide);
        Answer RemoveDisplayItem(string id);

    }
}
