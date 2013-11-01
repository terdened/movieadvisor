using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace movieAdvisor.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            if(User.IsInRole("admin"))
                return RedirectToAction("Index","Admin");
            
            ViewBag.Message = "Добро пожаловать на сайт Movie Advisor!";
            return View();
        }

        [Authorize]
        public ActionResult Private()
        {
           
            ViewBag.Message = "o_0";
            return View();
           
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
