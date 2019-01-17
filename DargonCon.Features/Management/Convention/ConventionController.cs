using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Features.Shared;
using DragonCon.Logical.Convention;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Internal;
using NodaTime;
using SystemClock = NodaTime.SystemClock;

namespace DragonCon.Features.Management.Convention
{
    [Area("Management")]
    public class ConventionController : DragonController<IConventionGateway>
    {
        private ConventionBuilder Builder;
        public ConventionController(
            ConventionBuilder builder,
            IConventionGateway gateway) : 
            base(gateway)
        {
            Builder = builder;
        }

        [HttpGet]
        public IActionResult Manage(int page = 0, int perPage = ResultsPerPage)
        {
            var conventionViewModel = Gateway.BuildConventionList(DisplayPagination.BuildForGateway(page, perPage));
            return View(conventionViewModel);
        }

        [HttpGet]
        public IActionResult CreateConvention()
        {
            return View(new ConventionCreateUpdateViewModel()
            {
                Days = new List<DaysViewModel>
                {
                }
            });
        }

        [HttpPost]
        public IActionResult CreateConvention(ConventionCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Name))
            {
                return RedirectToAction("CreateConvention", viewmodel);
            }

            var filteredList = viewmodel.Days.Where(x => x.IsDeleted == false).ToList();
            if (filteredList.Any() == false)
            {
                return RedirectToAction("CreateConvention", viewmodel);
            }

            var actualDays = ParseDays(filteredList);
            var builder = Builder.NewConvention(viewmodel.Name);
            foreach (var actualDay in actualDays)
            {
                builder.Days.AddDay(actualDay.Date, actualDay.StartTime, actualDay.EndTime);
                builder.Days.SetTimeSlotStrategy(actualDay.Date, TimeSlotStrategy.Exact246Windows);
            }

            builder.Save();
            return RedirectToAction("Manage");
        }

        private List<ConDay> ParseDays(List<DaysViewModel> days)
        {
            var results = new List<ConDay>();
            foreach (var day in days)
            {
                results.Add(new ConDay(
                    LocalDate.FromDateTime(day.Date), 
                    LocalTime.FromHourMinuteSecondTick(day.From.Hour, day.From.Minute, 0, 0),
                    LocalTime.FromHourMinuteSecondTick(day.To.Hour, day.To.Minute, 0, 0)));
            }

            return results;
        }

        [HttpPost]
        public Answer ToggleType(string type, bool value)
        {
            var config = Gateway.LoadSystemConfiguration();
            switch (type)
            {
                case "events":
                    config.AllowEventsSuggestions = value;
                    break;
                case "registration-add":
                    config.AllowEventsRegistration = value;
                    break;
                case "registration-change":
                    config.AllowEventsRegistrationChanges = value;
                    break;
                case "tickets":
                    config.AllowPayments = value;
                    break;
                default:
                    throw new Exception("Unknown config toggle");
            }
            Gateway.SaveSystemConfiguration(config);
            return Answer.Success;
        }

        [HttpPost]
        public Answer SetActive(string id)
        {
            var config = new SystemConfiguration
            {
                ActiveConventionId = id
            };

            Gateway.SaveSystemConfiguration(config);
            return Answer.Success;
        }
    }
}
