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
        public IActionResult CreateUpdateConvention(ConventionCreateUpdateViewModel viewmodel = null,
                                                    string conId = null)
        {
            if (string.IsNullOrWhiteSpace(conId) == false)
            {
                var convention = Builder.LoadConvention(conId);
                viewmodel = new ConventionCreateUpdateViewModel
                {
                    Id = conId,
                    Name = convention.ConventionName,
                    Days = convention.Days.AllDays.Select(x => new DaysViewModel(x)).ToList()
                };
            }

            if (viewmodel?.Days == null)
            {
                viewmodel = new ConventionCreateUpdateViewModel
                {
                    Days = new List<DaysViewModel>()
                };
            }

            return View("CreateUpdateConvention", viewmodel);
        }

        [HttpPost]
        public IActionResult CreateUpdateConventionPost(ConventionCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Id))
            {
                return CreateConvention(viewmodel);
            }
            else
            {
                return UpdateConvention(viewmodel);
            }
        }

        private IActionResult UpdateConvention(ConventionCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Name))
            {
                return CreateUpdateConvention(viewmodel);
            }

            var deletedFiltered = ParseDays(viewmodel.Days.Where(x => x.IsDeleted).ToList());
            var nonDeletedFiltered = ParseDays(viewmodel.Days.Where(x => x.IsDeleted == false).ToList());
            if (nonDeletedFiltered.Any() == false)
            {
                return CreateUpdateConvention(viewmodel);
            }

            var builder = Builder.LoadConvention(viewmodel.Id);
            foreach (var parsedDay in nonDeletedFiltered)
            {
                if (builder.Days.IsDaysExists(parsedDay.Date) == false)
                {
                    builder.Days.AddDay(parsedDay.Date, parsedDay.StartTime, parsedDay.EndTime);
                }
                else
                {
                    builder.Days.UpdateDay(parsedDay.Date, parsedDay.StartTime, parsedDay.EndTime);
                }

                builder.Days.SetTimeSlotStrategy(parsedDay.Date, TimeSlotStrategy.Exact246Windows);
            }

            foreach (var parsedDay in deletedFiltered)
            {
                if (nonDeletedFiltered.Any(x => x.Date == parsedDay.Date) == false)
                {
                    if (builder.Days.IsDaysExists(parsedDay.Date))
                        builder.Days.RemoveDay(parsedDay.Date);
                }
            }

            builder.ChangeName(viewmodel.Name);
            builder.Save();
            return RedirectToAction("Manage");        }

        private IActionResult CreateConvention(ConventionCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Name))
            {
                return CreateUpdateConvention(viewmodel);
            }

            var filteredList = viewmodel.Days.Where(x => x.IsDeleted == false).ToList();
            if (filteredList.Any() == false)
            {
                return CreateUpdateConvention(viewmodel);
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
