using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassProject.Models;

namespace ClassProject.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private ResultContext db = new ResultContext();

        public ActionResult Athlete(int? id)
        {
            if (id == null)
            {
                return View("~/Views/Error/PageNotFound.cshtml");
            }
            
            var found = db.Athletes.Find(id);

            if (found == null)
            {
                return View("~/Views/Error/PageNotFound.cshtml");
            }

            var ViewModel = new AthleteViewModel(found);

            return View(ViewModel);
        }
    }
}