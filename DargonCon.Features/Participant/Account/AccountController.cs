using System;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Features.Shared;
using DragonCon.Logical;
using DragonCon.Logical.Communication;
using DragonCon.Modeling.Models.Identities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
                    UserName = model.Email,
                    FullName = model.FullName,
                    Email = model.Email,
                    IsAllowingPromotions = model.IsAllowingPromotions,
                    PhoneNumber = model.PhoneNumber,
                };
                var addResult = await Identities.AddNewParticipant(participant, model.Password);
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
                    SetUserError("תקלה ביצירת משתתף", addResult.Errors.FirstOrDefault());
                    return View("LoginOrRegister", returnModel);
                }
            }
            else
            {
                var invalidProperty = ModelState.First(x => x.Value.ValidationState == ModelValidationState.Invalid);
                SetUserError("תקלה במידע שהתקבל", invalidProperty.Value.Errors.FirstOrDefault()?.ErrorMessage ?? "אנא נסו שוב");
                return View("LoginOrRegister", returnModel);

            }

            // If we got this far, something failed, redisplay form
            return View("LoginOrRegister", returnModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginViewModel model)
        {
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
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View("LoginOrRegister", model);
                }
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
            await Identities.LogoutAsync(Actor.Me.Id);
            return RedirectToAction("Login");
        }

        //[HttpGet]
        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _identity.GetUserByUsernameAsync(model.Email);
        //        if (user == null)
        //            return View("ForgotPasswordConfirmation");

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
        //        // Send an email with this link
        //        var result = await _identity.GeneratePasswordResetTokenAsync(user);
        //        var callbackUrl = Url.Action("ForgotPasswordCallback", "Account", new { userId = user.Id, code = result.Token }, HttpContext.Request.Scheme);
        //        await SendForgotPasswordEMail(user, callbackUrl);
        //        return View("ForgotPasswordConfirmation");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

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
