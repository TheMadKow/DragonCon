using System;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Features.Participant.Account;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Identities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DragonCon.Features.Participant.Personal
{
    [Area("Participant")]
    public class PersonalController : DragonController<IPersonalGateway>
    {
        public PersonalController(IServiceProvider service) : base(service) { }

        [HttpGet]
        public IActionResult Index()
        {
            var personalViewModel = Gateway.BuildPersonalViewModel();
            return View(personalViewModel);
        }

        #region Account Handling

        [HttpGet]
        public IActionResult UpdateAccount()
        {
            var user = Gateway.GetParticipant(Actor.Me.Id);
            var updateViewModel = new UpdateAccountViewModel();
            updateViewModel.Password = new PasswordChangeViewModel();
            updateViewModel.Details = new DetailsUpdateViewModel
            {
                YearOfBirth = user.YearOfBirth,
                FullName = user.FullName,
                Email = user.Email,
                IsAllowingPromotions = user.IsAllowingPromotions,
                PhoneNumber = user.PhoneNumber
            };
            return View(updateViewModel);
        }


        public async Task<IActionResult> ChangePassword(PasswordChangeViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var result = await Gateway.ChangePassword(viewmodel);
                var user = Gateway.GetParticipant(Actor.Me.Id);
                var updateViewModel = new UpdateAccountViewModel();
                updateViewModel.Password = new PasswordChangeViewModel();
                updateViewModel.Details = new DetailsUpdateViewModel
                {
                    YearOfBirth = user.YearOfBirth,
                    FullName = user.FullName,
                    Email = user.Email,
                    IsAllowingPromotions = user.IsAllowingPromotions,
                    PhoneNumber = user.PhoneNumber
                };

                if (result.AnswerType == AnswerType.Success)
                {
                    SetUserSuccess("שינוי סיסמה", "הסיסמה שונתה בהצלחה");
                    return View("UpdateAccount", updateViewModel);
                }
                else
                {
                    SetUserSuccess("שינוי סיסמה", "תקלה - " + result.Message);
                    return View("UpdateAccount", updateViewModel);
                }
            }
            else
            {
                var user = Gateway.GetParticipant(Actor.Me.Id);
                var updateViewModel = new UpdateAccountViewModel();
                updateViewModel.Password = new PasswordChangeViewModel();
                updateViewModel.Details = new DetailsUpdateViewModel
                {
                    YearOfBirth = user.YearOfBirth,
                    FullName = user.FullName,
                    Email = user.Email,
                    IsAllowingPromotions = user.IsAllowingPromotions,
                    PhoneNumber = user.PhoneNumber
                };
                SetUserError("תקלה במידע שהתקבל", ParseModelErrors());
                return View("UpdateAccount", updateViewModel);
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateDetails(DetailsUpdateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var updateViewModel = new UpdateAccountViewModel
                {
                    Password = new PasswordChangeViewModel(),
                    Details = viewModel
                };

                var result = await Gateway.UpdateDetails(viewModel);
                if (result.AnswerType == AnswerType.Success)
                {
                    SetUserSuccess("עדכון פרטים", "הפרטים עודכנו בהצלחה");
                    if (result.Message == "Logout")
                    {
                        return RedirectToAction("LoginOrRegister", "Account");
                    }
                    else
                    {
                        return View("UpdateAccount", updateViewModel);
                    }
                }
                else
                {
                    SetUserError("תקלה בעדכון פרטים", ParseModelErrors());
                    return View("UpdateAccount", updateViewModel);
                }
            }
            else
            {
                var updateViewModel = new UpdateAccountViewModel
                {
                    Password = new PasswordChangeViewModel(),
                    Details = viewModel
                };

                SetUserError("תקלה בעדכון פרטים", ParseModelErrors());
                return View("UpdateAccount", updateViewModel);
            }
        }

        #endregion

        #region Suggest Event

        [HttpGet]
        public IActionResult SuggestEvent()
        {
            return View(new SuggestEventViewModel());
        }

        public IActionResult SuggestEventPairing()
        {
            return View();
        }

        public IActionResult SuggestEventComplete()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SuggestEvent(SuggestEventViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var ans = Gateway.AddSuggestedEvent(viewmodel);
                if (ans.AnswerType == AnswerType.Error)
                {
                    SetUserError("תקלה בעדכון פרטים", ParseModelErrors());
                    return View(viewmodel);
                } 
                else
                {
                    return RedirectToAction("SuggestEventComplete");
                }
            }
            else
            {
                SetUserError("תקלה בעדכון פרטים", ParseModelErrors());
                return View(viewmodel);
            }
        }
        #endregion

        #region RegisterEvents

        public IActionResult EventsRegister()
        {
            var viewModel = Gateway.BuildEvents(Actor.Me.Id);
            return View(viewModel);
        }
        #endregion

        // TODO
        // Register (Me)
        // Register (Guest)
    }
}
