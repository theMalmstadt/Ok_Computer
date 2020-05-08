using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBM.DAL;
using OBM.Models;
using OBM.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Net;

namespace OBM.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string date = "2015-01-19T16:57:17-05:00";
            DateTime newDate = DateTime.Parse(date);

            EventContext db = new EventContext();
            var eventViewList = new List<EventViewModel>();
            if (Request.IsAuthenticated)
            {
                foreach (var i in db.Events.ToList())
                {
                    if (i.OrganizerID == User.Identity.GetUserId())
                        eventViewList.Add(new EventViewModel(i, HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(@i.OrganizerID).UserName));
                }
                ViewBag.Login = true;
            }
            else
            {
                ViewBag.Login = false;
                foreach (var i in db.Events.ToList())
                {
                    if (i.Public == true)
                        eventViewList.Add(new EventViewModel(i, HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(@i.OrganizerID).UserName));
                }
            }
            return View(eventViewList);
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Search(String search, String table)
        {
            ViewBag.search = WebUtility.HtmlEncode(search);
            ViewBag.table = table;
            return View();
        }
    }
}