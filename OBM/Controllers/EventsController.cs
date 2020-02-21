using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using OBM.Models;
using OBM.Models.ViewModels;
using OBM.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;

namespace OBM.Controllers
{

    public class EventsController : Controller
    {

        private EventContext db = new EventContext();

        // GET: Events
        public ActionResult Index()
        {
            var eventViewList = new List<EventViewModel>();
            foreach(var i in db.Events.ToList())
            {
                if(i.Public == true)
                    eventViewList.Add(new EventViewModel(i, HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(@i.OrganizerID).UserName));
            }
            return View(eventViewList);
        }

        public ActionResult Manage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eventView = new EventViewModel(db.Events.Find(id), HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(db.Events.Find(id).OrganizerID).UserName);
            if (eventView == null)
            {
                return HttpNotFound();
            }
            if ((Request.IsAuthenticated && (User.Identity.GetUserId() == eventView.OrganizerID)))
            {
                ViewBag.Access = true;
            }
            else
                ViewBag.Access = false;

            return View(eventView);
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eventView = new EventViewModel(db.Events.Find(id), HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(db.Events.Find(id).OrganizerID).UserName);
            if (eventView == null)
            {
                return HttpNotFound();
            }
            if ((eventView.Public == true) || (Request.IsAuthenticated && (User.Identity.GetUserId() == eventView.OrganizerID)))
            {
                ViewBag.Access = true;
            }
            else
                ViewBag.Access = false;
            
            return View(eventView);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            ViewBag.OrganizerID = User.Identity.GetUserId();
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,OrganizerID,EventName,Description,Location,Public")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.OrganizerID = User.Identity.GetUserId();
            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            if ((Request.IsAuthenticated && (User.Identity.GetUserId() == @event.OrganizerID)))
            {
                ViewBag.Access = true;
            }
            else
                ViewBag.Access = false;
            ViewBag.OrganizerID = User.Identity.GetUserId();
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,OrganizerID,EventName,Description,Location,Public")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if ((Request.IsAuthenticated && (User.Identity.GetUserId() == @event.OrganizerID)))
            {
                ViewBag.Access = true;
            }
            else
                ViewBag.Access = false;
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            if ((Request.IsAuthenticated && (User.Identity.GetUserId() == @event.OrganizerID)))
            {
                ViewBag.Access = true;
            }
            else
                ViewBag.Access = false;
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult GetTournament()
        {
            //string challongUsername = "sudo_whoami";
            string test_api_key = "AHbBLmooY7VFlkmGO7DmMUii8tfHWAkDCy4ubAR3";
            string myResponse = "";

            var search = Request.QueryString["search"];
            ViewBag.Found = "";
            if (search == string.Empty)
            {
                ViewBag.Found = "No search term entered.";
            }
            else if (search != null)
            {
                var searchSegments = new Uri(search).Segments;
// NEED TO ADD ERROR CHECKING FOR BAD SEARCH STRINGS
                if (searchSegments != null)
                {
                    string tournamentRoute = searchSegments[searchSegments.Length - 1];
                    tournamentRoute = @"https://api.challonge.com/v1/tournaments/" + tournamentRoute + ".json?api_key=" + test_api_key;
                    ViewBag.Found = tournamentRoute;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tournamentRoute);
                    request.Method = "Get";
                    request.Headers.Add("api_key", test_api_key);
                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                        {
                            myResponse = sr.ReadToEnd();
                        }
                        ViewBag.response = myResponse;
                    }
                    catch { }
// NEED TO ADD ERROR CHECKING FOR BAD REQUESTS
                }

                JObject jsonResponse = JObject.Parse(myResponse);
                //ViewBag.Found = tournamentName + tournamentDesc + tournamentGame + tournamentAPI + tournamentURL + tournamentTeams;

                
                Tournament newTournament = new Tournament();
                newTournament.TournamentName = (string)jsonResponse["tournament"]["name"];
                newTournament.EventID = 1;
                newTournament.Description = (string)jsonResponse["tournament"]["description"];
                newTournament.Game = (string)jsonResponse["tournament"]["game_name"];
                newTournament.ApiId = null;
                newTournament.UrlString = (string)jsonResponse["tournament"]["url"];
                newTournament.IsTeams = (bool)jsonResponse["tournament"]["teams"];

                if (ModelState.IsValid)
                {
                    EventContext DB = new EventContext();
                    DB.Tournaments.Add(newTournament);
                    DB.SaveChanges();
                    ViewBag.Success = "Tournament was added.";
                }
            }

            return View();
        }
    }
}
