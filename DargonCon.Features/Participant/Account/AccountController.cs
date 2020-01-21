using System;
using System.Threading.Tasks;
using DragonCon.Features.Shared;
using DragonCon.Logical.Communication;
using DragonCon.Logical.Identities;
using DragonCon.Modeling.Models.Identities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DragonCon.Features.Participant.Account
{
    [Area("Participant")]
    [AllowAnonymous]
    public class AccountController : DragonController<NullGateway>
    {
        public IIdentityFacade Identities { get; set; }
        public ICommunicationHub Communication { get; set; }

        public AccountController(IServiceProvider service) :
            base(service)
        {
            Identities = service.GetRequiredService<IIdentityFacade>();
            Communication = service.GetRequiredService<ICommunicationHub>();
        }

        public IActionResult LoginOrRegister()
        {
            return View(new AccountViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterViewModel model)
        {
            var returnModel = new AccountViewModel()
            {
                Register = model
            };

            if (ModelState.IsValid)
            {
                var participant = new LongTermParticipant
                {
                    YearOfBirth = model.YearOfBirth,
                    FullName = model.FullName,
                    Email = model.Email,
                    IsAllowingPromotions = model.IsAllowingPromotions,
                    PhoneNumber = model.PhoneNumber,
                };
                var addResult = await Identities.AddNewParticipant(participant, model.Password);
                if (addResult.IsSuccess && addResult.IsLongTerm == false)
                {
                    SetUserError("כשלון ביצירת משתמש ארוך טווח", addResult.Details);
                    return View("LoginOrRegister", returnModel);
                }
                if (addResult.IsSuccess)
                {
                    await Communication.SendWelcomeMessageAsync(participant);
                    var signInResult = await Identities.LoginAsync(model.Email, model.Password, true);
                    if (signInResult.IsSuccess)
                    {
                        var cookieConsent = HttpContext.Features.Get<ITrackingConsentFeature>();
                        cookieConsent.GrantConsent();
                        return RedirectToAction("Index", "Personal", new { area = "Participant" });
                    }
                    else
                    {
                        SetUserError("כשלון בהתחברות לאחר רישום", signInResult.Details);
                        return View("LoginOrRegister", returnModel);
                    }
                }
                else
                {
                    SetUserError("תקלה ביצירת משתתף", addResult.Details);
                    return View("LoginOrRegister", returnModel);
                }
            }
            else
            {
                SetUserError("תקלה במידע שהתקבל", ParseModelErrors());
                return View("LoginOrRegister", returnModel);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginViewModel model)
        {
            var returnModel = new AccountViewModel()
            {
                Login = model
            };
            
            if (ModelState.IsValid)
            {
                var result = await Identities.LoginAsync(model.Email, model.Password, true);
                if (result.IsSuccess)
                {
                    var cookieConsent = HttpContext.Features.Get<ITrackingConsentFeature>();
                    cookieConsent.GrantConsent();
                    return RedirectToAction("Index", "Personal", new { area = "Participant" });
                }
                else
                {
                    SetUserError("כשלון בהתחברות ", result.Details);
                    return View("LoginOrRegister", returnModel);
                }
            }
            else
            {
                SetUserError("תקלה במידע שהתקבל", ParseModelErrors());
                return View("LoginOrRegister", returnModel);

            }

            // If we got this far, something failed, redisplay form
            return View("LoginOrRegister", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var myCookies = Request.Cookies.Keys;
            foreach (var cookie in myCookies)
                Response.Cookies.Delete(cookie);
            await Identities.LogoutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (ModelState.IsValid)
            {
                var user = await Identities.GetParticipantByUsernameAsync(email);
                if (user == null)
                    return RedirectToAction("ForgotPasswordConfirm");

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var result = await Identities.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ForgotPasswordCallback", "Account", new { userId = user.Id, code = result.Token }, HttpContext.Request.Scheme);
                await Communication.SendForgotPassword(user, callbackUrl);
                return RedirectToAction("ForgotPasswordConfirm");
            }
            else
            {
                SetUserError("תקלה במידע שהתקבל", ParseModelErrors());
                return View();
            }
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirm()
        {
            return View();
        }


        //private async Task SendForgotPasswordEMail(ISystemUser user, string callbackUrl)
        //{
        //    var request = Templates.Email.GeneralEmail
        //        .SetTiming(CommunicationTiming.Immediate)
        //        .SetRecipient(user.Email, user.FullName)
        //        .SetUser(user.Id)
        //        .SetSubject(Repository.Parameters["Controller.Personal.Account.Forgot-Password.Title"])
        //        .SetContent(Repository.Parameters.Formatted("Controller.Personal.Account.Forgot-Password.Body", callbackUrl))
        //        .SetSpecial("SendIfNotActivated")
        //        .Build();
        //    await _communications.AddRequest(await request);
        //}

        //[HttpGet]
        //public IActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// GET: /Users/ResetPassword
        //[HttpGet]
        //public IActionResult ForgotPasswordCallback(string code = null)
        //{
        //    if (code == null)
        //        return RedirectToAction("Error", "Home", AreaName(""));

        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> ForgotPasswordCallback(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);
        //    var user = await _identity.GetUserByUsernameAsync(model.Email);
        //    if (user == null)
        //        return RedirectToAction("ForgotPasswordCallbackConfirmation", "Account");
        //    var result = await _identity.ResetPasswordAsync(user, model.Code, model.Password);
        //    if (result.IsSuccess)
        //    {
        //        await SendForgotPasswordConfirmationEmail(user);
        //        return RedirectToAction("ForgotPasswordCallbackConfirmation", "Account");
        //    }

        //    AddModelErrors(result);
        //    return View();
        //}

        //private async Task SendForgotPasswordConfirmationEmail(ISystemUser user)
        //{
        //    var request = Templates.Email.GeneralEmail
        //        .SetTiming(CommunicationTiming.Immediate)
        //        .SetRecipient(user.Email, user.FullName)
        //        .SetUser(user.Id)
        //        .SetSubject(Repository.Parameters["Controller.Personal.Account.Forgot-Password-Confirmation.Title"])
        //        .SetContent(Repository.Parameters["Controller.Personal.Account.Forgot-Password-Confirmation.Body"])
        //        .SetSpecial("SendIfNotActivated")
        //        .Build();
        //    await _communications.AddRequest(await request);
        //}

        //[HttpGet]
        //public IActionResult ForgotPasswordCallbackConfirmation()
        //{
        //    return View();
        //}
    }
}
