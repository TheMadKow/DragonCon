using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Features.Shared;
using DragonCon.Logical.Convention;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.System;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace DragonCon.Features.Management.Convention
{
    [Area("Management")]
    public class ConventionController : DragonController<IManagementConventionGateway>
    {
        private ConventionBuilder Builder;
        public ConventionController(
            ConventionBuilder builder,
            IManagementConventionGateway gateway) : 
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
        public IActionResult ShowDetails(string id)
        {
            var conUpdateViewModel = Gateway.BuildConventionUpdate(id);
            return View("ShowDetails", conUpdateViewModel);
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

        #region Name & Days
        [HttpPost]
        public IActionResult CreateUpdateNameDatePost(NameDatesCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Id))
                return CreateConventionPost(viewmodel);
            else 
                return UpdateNameDates(viewmodel);
        }

        [HttpGet]
        public IActionResult CreateConvention(NameDatesCreateUpdateViewModel viewmodel = null)
        {
            if (viewmodel == null)
            {
                viewmodel = new NameDatesCreateUpdateViewModel();
            }

            if (viewmodel.Days == null)
            {
                viewmodel.Days = new List<DaysViewModel>();
            }

            viewmodel.Days.Add(new DaysViewModel()
            {
                Date = DateTime.Today,
                From = new DateTime(1,1,1, 9, 0, 0, DateTimeKind.Unspecified),
                To = new DateTime(1, 1, 1, 21, 0, 0, DateTimeKind.Unspecified)
            });
            return View("CreateUpdateNameDates", viewmodel);
        }


        [HttpPost]
        public IActionResult CreateConventionPost(NameDatesCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Name))
            {
                return CreateConvention(viewmodel);
            }

            var filteredList = viewmodel.Days.Where(x => x.IsDeleted == false).ToList();
            if (filteredList.Any() == false)
            {
                return CreateConvention(viewmodel);
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
        
        private List<Day> ParseDays(List<DaysViewModel> days)
        {
            var results = new List<Day>();
            foreach (var day in days)
            {
                results.Add(new Day(
                    LocalDate.FromDateTime(day.Date), 
                    LocalTime.FromHourMinuteSecondTick(day.From.Hour, day.From.Minute, 0, 0),
                    LocalTime.FromHourMinuteSecondTick(day.To.Hour, day.To.Minute, 0, 0)));
            }

            return results;
        }
        
        [HttpGet]
        public IActionResult UpdateConvention(string conId, string activeTab = null, string errorMessage = null)
        {
            ViewBag.HelperTab = activeTab;
            var conUpdateViewModel = Gateway.BuildConventionUpdate(conId);
            conUpdateViewModel.ErrorMessage = errorMessage;
            return View("UpdateConvention", conUpdateViewModel);
        }

        
        [HttpPost]
        public IActionResult UpdateNameDates(NameDatesCreateUpdateViewModel viewmodel)
        {
            if (string.IsNullOrWhiteSpace(viewmodel.Name))
            {
                return RedirectToAction("UpdateConvention", new
                {
                    conId = viewmodel.Id,
                    activeTab = "days",
                    errorMessage = "הוזן שם כנס ריק"
                });
            }

            var deletedFiltered = ParseDays(viewmodel.Days.Where(x => x.IsDeleted).ToList());
            var nonDeletedFiltered = ParseDays(viewmodel.Days.Where(x => x.IsDeleted == false).ToList());
            if (nonDeletedFiltered.Any() == false)
            {
                return RedirectToAction("UpdateConvention", new
                {
                    conId = viewmodel.Id,
                    activeTab = "days",
                    errorMessage = "לא הוזנו ימים לכנס"
                });
            }
            if (nonDeletedFiltered.GroupBy(x => x.Date).Any(x => x.Count() > 1))
            {
                return RedirectToAction("UpdateConvention", new
                {
                    conId = viewmodel.Id,
                    activeTab = "days",
                    errorMessage = "הוזן אותו התאריך יותר מפעם אחת"
                });
            }
            try
            {
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
            }
            catch (Exception e)
            {
                return RedirectToAction("UpdateConvention", new
                {
                    conId = viewmodel.Id,
                    activeTab = "days",
                    errorMessage = e.Message
                });
            }

            return RedirectToAction("UpdateConvention", new
            {
                conId = viewmodel.Id,
                activeTab = "days",
            });
        }

        #endregion
        
        #region Halls

        [HttpPost]
        public IActionResult CreateUpdateHalls(HallsUpdateViewModel viewmodel)
        {
            var deletedFiltered = viewmodel.Halls.Where(x => x.IsDeleted).ToList();
            var nonDeletedFiltered = viewmodel.Halls.Where(x => x.IsDeleted == false).ToList();
            if (nonDeletedFiltered.Any() == false)
            {
                return RedirectToAction("UpdateConvention", new
                {
                    conId = viewmodel.ConventionId,
                    activeTab = "halls",
                    errorMessage = "לא הוזנו אולמות לכנס"
                });
            }

            if (nonDeletedFiltered.Any(x => string.IsNullOrWhiteSpace(x.Name)))
            {
                return RedirectToAction("UpdateConvention", new
                {
                    conId = viewmodel.ConventionId,
                    activeTab = "halls",
                    errorMessage = "הוזן שם אולם ריק"
                });

            }

            try
            {
                var builder = Builder.LoadConvention(viewmodel.ConventionId);
                foreach (var hall in deletedFiltered)
                {
                    if (nonDeletedFiltered.Any() == false)
                    {
                        if (builder.Halls.KeyExists(hall.Id))
                            builder.Halls.RemoveHall(hall.Id);
                    }
                }

                foreach (var hall in nonDeletedFiltered)
                {
                    if (builder.Halls.KeyExists(hall.Id) == false)
                    {
                        builder.Halls.AddHall(hall.Name, hall.Description, hall.FirstTable, hall.LastTable);
                    }
                    else
                    {
                        builder.Halls.UpdateHall(hall.Id,
                            hall.Name, hall.Description, hall.FirstTable, hall.LastTable);
                    }
                }

                builder.Save();
            }
            catch (Exception e)
            {
                return RedirectToAction("UpdateConvention", new
                {
                    conId = viewmodel.ConventionId,
                    activeTab = "halls",
                    errorMessage = e.Message
                });
            }

            return RedirectToAction("UpdateConvention", new
            {
                activeTab = "halls",
                conId = viewmodel.ConventionId,
            });
        }
        #endregion

        #region Tickets

        [HttpPost]
        public IActionResult CreateUpdateTickets(TicketsUpdateViewModel viewmodel)
        {
            var deletedFiltered = viewmodel.Tickets.Where(x => x.IsDeleted).ToList();
            var nonDeletedFiltered = viewmodel.Tickets.Where(x => x.IsDeleted == false).ToList();
            if (nonDeletedFiltered.Any() == false)
            {
                return RedirectToAction("UpdateConvention", new
                {
                    conId = viewmodel.ConventionId,
                    activeTab = "tickets",
                    errorMessage = "לא הוזנו כרטיסים לכנס"
                });
            }

            if (nonDeletedFiltered.Any(x => string.IsNullOrWhiteSpace(x.Name)))
            {
                return RedirectToAction("UpdateConvention", new
                {
                    conId = viewmodel.ConventionId,
                    activeTab = "tickets",
                    errorMessage = "הוזן שם כרטיס ריק"
                });
            }

            if (nonDeletedFiltered.Any(x => x.Days.Any() == false))
            {
                return RedirectToAction("UpdateConvention", new
                {
                    conId = viewmodel.ConventionId,
                    activeTab = "tickets",
                    errorMessage = "הוזן כרטיס ללא שיוך לימי כנס"
                });
            }

            if (nonDeletedFiltered.Any(x => x.Price < 0))
            {
                return RedirectToAction("UpdateConvention", new
                {
                    conId = viewmodel.ConventionId,
                    activeTab = "tickets",
                    errorMessage = "הוזן כרטיס עם מחיר שלילי"
                });
            }

            try
            {
                var builder = Builder.LoadConvention(viewmodel.ConventionId);
                foreach (var ticket in deletedFiltered)
                {
                    if (nonDeletedFiltered.Any() == false)
                    {
                        if (builder.Tickets.KeyExists(ticket.Id))
                            builder.Tickets.RemoveTicket(ticket.Id);
                    }
                }

                foreach (var ticket in nonDeletedFiltered)
                {
                    if (builder.Tickets.KeyExists(ticket.Id) == false)
                    {
                        builder.Tickets.AddTicket(
                            ticket.Name,
                            ticket.Days,
                            ticket.TransactionCode,
                            ticket.IsLimited ? ticket.NumOfActivities : null,
                            ticket.Price,
                            ticket.TicketType);
                    }
                    else
                    {
                        builder.Tickets.UpdateTicket(ticket.Id,
                            ticket.Name,
                            ticket.Days,
                            ticket.TransactionCode,
                            ticket.IsLimited ? ticket.NumOfActivities : null,
                            ticket.Price,
                            ticket.TicketType);
                    }
                }

                builder.Save();
            }
            catch (Exception e)
            {
                return RedirectToAction("UpdateConvention", new
                {
                    conId = viewmodel.ConventionId,
                    activeTab = "tickets",
                    errorMessage = e.Message
                });
            }

            return RedirectToAction("UpdateConvention", new
            {
                activeTab = "tickets",
                conId = viewmodel.ConventionId,
            });
        }

        #endregion
    }
}
