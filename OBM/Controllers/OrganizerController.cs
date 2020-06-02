using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBM.DAL;
using OBM.Models;
using OBM.Models.ViewModels;

namespace OBM.Controllers
{
    public class OrganizerController : Controller
    {
        EventContext db = new EventContext();
        // GET: Organizer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrganizersSearch()
        {
            List<Organizer> organizers = new List<Organizer>();

            foreach(var user in db.AspNetUsers)
            {
                Organizer organizer = new Organizer();

                organizer.organizerName = user.UserName;
                organizer.organizerID = user.Id;

                organizers.Add(organizer);
            }

            return View(organizers);
        }

        [HttpGet]
        public ActionResult PublicProfile(string OrgID)
        {
            Organizer organizer = new Organizer();

            organizer.organizerID = OrgID;
            organizer.organizerName = db.AspNetUsers.Find(OrgID).UserName;

            OrganizerViewModel organizerViewModel = new OrganizerViewModel(organizer);

            return View(organizerViewModel);
        }
    }
}