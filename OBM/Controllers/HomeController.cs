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

namespace OBM.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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
    }
}