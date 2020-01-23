using System;
using DragonCon.Features.Shared;
using DragonCon.Modeling.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace DragonCon.Features.Convention.Home
{
    [Area("Convention")]
    public class HomeController : DragonController<IDisplayPublicGateway>
    {
        public HomeController(IServiceProvider service) : 
            base(service)  { }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Info()
        {
            return View();
        }

        public IActionResult Volunteering()
        {
            return View();
        }
        public IActionResult VolunteeringForm()
        {
            return View();
        }
        public IActionResult English()
        {
            return View();
        }
        public IActionResult Events()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ContactUs(ContactUsViewModel viewModel)
        {
            var answer = Gateway.SendContactUs(viewModel);
            if (answer.AnswerType != AnswerType.Success)
            {
                if (viewModel.IsEnglish)
                {
                    SetUserError("Failure to Contact", answer.Message);
                    return RedirectToAction("English");
                } 
                else
                {
                    SetUserError("לא יכול ליצור קשר", answer.Message);
                    return RedirectToAction("Index");
                }
            }

            if (viewModel.IsEnglish)
            {
                SetUserSuccess("Contact Established", "Your request was sent");
                return RedirectToAction("English");
            }
            else
            {
                SetUserSuccess("נוצר קשר", "הבקשה שלך נשלחה");
                return RedirectToAction("Index");
            }
        }

        public IActionResult About()
        {
            var viewModel = Gateway.BuildAbout();
            return View(viewModel);
        }
        public IActionResult AboutStorms()
        {
            return View();
        }

        public IActionResult SiteMap()
        {
            return View();
        }

        public IActionResult Usage()
        {
            return View();
        }

        public IActionResult Error(string err)
        {
            return View();
        }

        public IActionResult Updates()
        {
            return View();
        }
    }
}
