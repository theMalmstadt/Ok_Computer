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
using System.Net.Http;
using System.Diagnostics;
using System.IO;

namespace OBM.Controllers
{

    public class EventsController : Controller
    {

        private EventContext db = new EventContext();

        // GET: Events
        public ActionResult Index()
        {
            var eventViewList = new List<EventViewModel>();
            foreach (var i in db.Events.ToList())
            {
                if (i.Public == true)
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
        public ActionResult NewTournament(int? id)
        {
            if (id == null)
            {
                //throw new HttpException(404, "No Page Found");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (db.Events.Find(id).OrganizerID == User.Identity.GetUserId())
            {
                ViewBag.Access = true;
                var challongURL = Request.QueryString["search"];
                //var api_key = Request.QueryString["api_key"];
                var api_key = HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(User.Identity.GetUserId()).ApiKey;
                ViewBag.Found = api_key;

                ViewBag.Success = "";

                if (challongURL == string.Empty)
                {
                    ViewBag.Success = "No url entered.";
                }
                else if (api_key == null || api_key == string.Empty)
                {
                    ViewBag.Success = "No api key associated with your account. Don't have one? You can find yours ";
                    ViewBag.Link1 = "https://challonge.com/settings/developer";
                    ViewBag.Link2 = "here.";
                }
                else if (challongURL != null && api_key != null)
                {
                    if (!Uri.IsWellFormedUriString(challongURL, UriKind.Absolute))
                    {
                        ViewBag.Success = "Invalid URL";
                    }
                    else
                    {
                        var searchSegments = new Uri(challongURL).Segments;
                        if (searchSegments != null)
                        {
                            string urlEnd = searchSegments[searchSegments.Length - 1];
                            string tournamentRoute = @"https://api.challonge.com/v1/tournaments/" + urlEnd + ".json?api_key=" + api_key;
                            //ViewBag.Found = participantsRoute;
                            string responseTournament = "";

                            try
                            {
                                HttpWebRequest requestTournaments = (HttpWebRequest)WebRequest.Create(tournamentRoute);
                                requestTournaments.Method = "Get";
                                requestTournaments.Headers.Add("api_key", api_key);
                                HttpWebResponse response1 = (HttpWebResponse)requestTournaments.GetResponse();
                                using (System.IO.StreamReader sr = new System.IO.StreamReader(response1.GetResponseStream()))
                                {
                                    responseTournament = sr.ReadToEnd();
                                }

                                JObject jsonTournament = JObject.Parse(responseTournament);
                                Tournament newTournament = new Tournament
                                {
                                    TournamentName = (string)jsonTournament["tournament"]["name"],
                                    EventID = id ?? default,
                                    Description = (string)jsonTournament["tournament"]["description"],
                                    Game = (string)jsonTournament["tournament"]["game_name"],
                                    ApiId = (int)jsonTournament["tournament"]["id"],
                                    UrlString = (string)jsonTournament["tournament"]["url"],
                                    IsTeams = (bool)jsonTournament["tournament"]["teams"]
                                };

                                if (ModelState.IsValid)
                                {
                                    EventContext DB = new EventContext();
                                    DB.Tournaments.Add(newTournament);
                                    DB.SaveChanges();
                                    ViewBag.Success = "Tournament was added ";
                                    ViewBag.Link1 = "/Events/Tournament/" + newTournament.TournamentID;
                                    ViewBag.Link2 = "here.";
                                    //Add link here to event page with tournament showing
                                }
                                else
                                {
                                    ViewBag.Success = "Tournament was not saved";
                                }
                            }
                            catch
                            {
                                ViewBag.Success = "Unable to get tournament data. Please review and re-enter the URL and Api Key above.";
                            }
                        }
                    }
                }
            }
            else
                ViewBag.Access = false;

            return View();
        }

        public ActionResult Tournament(int? id)
        {
            if (id == null)
            {
                //throw new HttpException(404, "No Page Found");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                Tournament found = db.Tournaments.Find(id);
                TournamentViewModel tour = new TournamentViewModel(found, HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(db.Events.Find(found.EventID).OrganizerID).UserName);
            
                if (tour == null)
                {
                    return HttpNotFound();
                }
                if ((tour.Public == true) || (Request.IsAuthenticated && (User.Identity.GetUserId() == db.Events.Find(found.EventID).OrganizerID)))
                {
                    ViewBag.Access = true;
                }
                else
                {
                    ViewBag.Access = false;
                }
                return View(tour);

            }
            catch 
            {
                return HttpNotFound();
            }
        }

        public void CompetitorUpdate(int? id)
        {
            //Temp
            string api_key = "vPWOezVjTtWNndYKvUMilHKz7PbhNukG1A3byil2";
            foreach (var i in db.Tournaments.Where(p => p.EventID == id).ToList())
            {
                string uri = "https://api.challonge.com/v1/tournaments/" + i.ApiId + "/participants.json?api_key=" + api_key;
                string data = SendRequest(uri);
                var participantsObject = JToken.Parse(data);
                foreach (var p in participantsObject)
                {
                    Boolean InDB = false;
                    var participant = (string)p["participant"]["name"];
                    foreach (var c in db.Competitors.Where(x => x.EventID == id))
                    {
                        if (c.CompetitorName == participant)
                        {
                            InDB = true;
                            break;
                        }
                    }
                    if (InDB == false)
                    {
                        Competitor newCompetitor = new Competitor
                        {
                            CompetitorName = participant,
                            EventID = id ?? default,
                            BusyState = null
                        };
                        db.Competitors.Add(newCompetitor);
                        db.SaveChanges();
                    }

                }
            }
        }

        private string SendRequest(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Accept = "application/json";

            string jsonString = null;
            // TODO: You should handle exceptions here
            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                jsonString = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            return jsonString;
        }

        public JsonResult CompetitorList(int? id)
        {
            CompetitorUpdate(id);
            string compStr = "<table class=\"table table-bordered table - striped\"><tr><th>Competitors</th></tr>";
            foreach (var i in db.Competitors.Where(p => p.EventID == id).ToList().OrderBy(p => p.CompetitorName))
            {
                compStr += "<tr><td>" + i.CompetitorName + "</td></tr>";
            }
            compStr += "</table>";
            var data = new
            {
                compTable = compStr
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
