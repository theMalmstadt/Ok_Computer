using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Diagnostics;
using System.Web.Mvc;
using Newtonsoft.Json;
using OBM.DAL;
using OBM.Models;
using OBM.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.IO;
using static MoreLinq.Extensions.MaxByExtension;
using static MoreLinq.Extensions.MinByExtension;

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
                throw new HttpException(400, "Bad Request");
            }
            var eventView = new EventViewModel(db.Events.Find(id), HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(db.Events.Find(id).OrganizerID).UserName);
            if (eventView == null)
            {
                throw new HttpException(404, "Page not Found");
            }
            if ((Request.IsAuthenticated && (User.Identity.GetUserId() == eventView.OrganizerID)))
            {
                ViewBag.Access = true;
            }
            else
                ViewBag.Access = false;

            return View(eventView);
        }

        [HttpGet]
        public ActionResult EventSearch(String search)
        {
            ViewBag.search = search;
            return View(db.Events.Where(x=>x.EventName.Contains(search) && x.Public).ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                throw new HttpException(400, "Bad Request");
            }
            var eventView = new EventViewModel(db.Events.Find(id), HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(db.Events.Find(id).OrganizerID).UserName);
            if (eventView == null)
            {
                throw new HttpException(404, "Page not Found");
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
                throw new HttpException(400, "Bad Request");
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                throw new HttpException(404, "Page not Found");
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
                throw new HttpException(400, "Bad Request");
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                throw new HttpException(404, "Page not Found");
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

        public ActionResult ResponsiveEvents(String search)
        {
            ViewBag.search = search;
            return View();
        }

        [HttpGet]
        public JsonResult EventList(String location, String name)   //ONCE AN EVENT DATE IS PROPERLY IMPLEMENTED IN THE DATABASE, IT WILL BE USED TO SORT THE RESULTS OF THESE QUERIES (.Orderby(x=>x.DateTime))
        {
            Debug.WriteLine(location, name);

            List<Event> eventList = new List<Event>();
            JArray TournamentList = new JArray();

            Debug.WriteLine("LOCATION IS: " + location);
            Debug.WriteLine("NAME IS: " + name);
            if (location != null&&location!="")// if we know the location of event, get the events at the location
            {
                foreach (var i in db.Events.Where(p => p.Public && p.Location.Contains(location)).ToList())
                {
                    eventList.Add(i);
                }
            }

            else// if the event field is null, or empty, return all events
            {
                foreach (var i in db.Events.Where(p => p.Public).ToList())
                {
                    eventList.Add(i);
                }
            }

            //NOW THE SEARCH BY NAME
            if (name != null && name != "")
            {
                var eventListTemp =new List<Event>(eventList);
                foreach (var i in eventListTemp)
                {
                    if(!i.EventName.Contains(name))
                    {
                        eventList.Remove(i);
                    }
                }
            }

            return Json (JsonConvert.SerializeObject(eventList, Formatting.Indented), JsonRequestBehavior.AllowGet);
        }

        public JsonResult TournamentList(int? id)
        {
            List<Tournament> TournamentList = new List<Tournament>();

            foreach (var i in db.Tournaments.Where(x=>x.EventID==id).ToList())
            {
                TournamentList.Add(i);
            }

            return Json(JsonConvert.SerializeObject(TournamentList, Formatting.Indented), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult NewTournament(int? id)
        {
            if (id == null)
            {
                throw new HttpException(400, "Bad Request");
            }
            else if (db.Events.Find(id).OrganizerID == User.Identity.GetUserId())
            {
                ViewBag.Access = true;
                var challongURL = Request.QueryString["search"];
                //var api_key = Request.QueryString["api_key"];
                var api_key = HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(User.Identity.GetUserId()).ApiKey;
                ViewBag.Found = api_key;
                ViewBag.api_key = api_key;
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
                            ViewBag.Found = api_key;
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
                                    EventID = id ?? default(int),
                                    Description = (string)jsonTournament["tournament"]["description"],
                                    Game = (string)jsonTournament["tournament"]["game_name"],
                                    ApiId = (int)jsonTournament["tournament"]["id"],
                                    UrlString = (string)jsonTournament["tournament"]["url"],
                                    IsTeams = (bool)jsonTournament["tournament"]["teams"],
                                    IsStarted = false
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

        [HttpGet]
        public ActionResult Tournament(int? id)
        {
            if (id == null)
            {
                throw new HttpException(404, "Page not Found");
            }

            try
            {
                Tournament found = db.Tournaments.Find(id);
                TournamentViewModel tour = new TournamentViewModel(found, HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(db.Events.Find(found.EventID).OrganizerID).UserName);
            
                if (tour == null)
                {
                    throw new HttpException(404, "Page not Found");
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
                throw new HttpException(404, "Page not Found");
            }
        }



        [HttpPost]
        public ActionResult Tournament(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Tournament found = db.Tournaments.Find(id);
                    db.Tournaments.Remove(found);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                throw new HttpException(401, "Tournament does not exist");
            }
        }


        [HttpGet]
        public ActionResult Competitor(int? id)
        {
            if (id == null)
            {
                throw new HttpException(404, "Page not Found");
            }

            try
            {
                Competitor comp = db.Competitors.Find(id);
                Event even = db.Events.Find(comp.EventID);
                if (comp == null)
                {
                    throw new HttpException(404, "Page not Found");
                }
                if ((even.Public == true) || (Request.IsAuthenticated && (User.Identity.GetUserId() == even.OrganizerID)))
                {
                    ViewBag.Access = true;
                }
                else
                {
                    ViewBag.Access = false;
                }
                return View(comp);
            }
            catch
            {
                throw new HttpException(404, "Page not Found");
            }
        }

        [HttpPost]
        public JsonResult Competitor(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var found = db.Competitors.Find(id);
                    if (found.BusyState == "a")
                    {
                        found.BusyState = "b";
                    }
                    else
                    {
                        found.BusyState = "a";
                    }
                    db.SaveChanges();
                }
                return Json(new { success = true, responseText = "Successful Post" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false, responseText = "Bad Post Request" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Matches(int? id)
        {
            List<Tournament> tournList = db.Tournaments.Where(x => x.EventID == id).ToList();
            List<Match> matchList = new List<Match>();
            foreach (var tourn in tournList)
            {
                List<Match> temp = db.Matches.Where(x => x.TournamentID == tourn.TournamentID).ToList();
                matchList.AddRange(temp);
            }

            List<MatchViewModel> matchVM = new List<MatchViewModel>();
            foreach (var model in matchList)
            {
                matchVM.Add(new MatchViewModel(model));
            }

            List<MatchViewModel> sortedList = matchVM.OrderBy(x => x.Status).ThenBy(y => y.Time).ToList();

            return Json(sortedList, JsonRequestBehavior.AllowGet);
        }

        public void CompetitorUpdate(int? id)
        {
            string api_key = HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(db.Events.Find(id).OrganizerID).ApiKey;
            foreach (var i in db.Tournaments.Where(p => p.EventID == id).ToList())
            {
                if(i.IsStarted == false)
                {
                    string uri = "https://api.challonge.com/v1/tournaments/" + i.ApiId + "/participants.json?api_key=" + api_key;
                    string participantsData = SendRequest(uri);
                    var participantsObject = JToken.Parse(participantsData);
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
                                EventID = id ?? default(int),
                                BusyState = null
                            };
                            db.Competitors.Add(newCompetitor);
                            db.SaveChanges();//Move to save only once?
                        }

                    }
                    uri = "https://api.challonge.com/v1/tournaments/" + i.ApiId + ".json?api_key=" + api_key;
                    string startData = SendRequest(uri);
                    var startObject = JToken.Parse(startData);
                    if (startObject["tournament"]["started_at"].ToString() != "")
                    {
                        MatchSetup(i.TournamentID, id, participantsObject);
                        i.IsStarted = true;
                        db.SaveChanges();
                    }
                }   
            }
        }

        public void MatchSetup(int? tid, int? eid, JToken participantsObject)
        {
            string api_key = HttpContext.GetOwinContext().Get<ApplicationUserManager>().FindById(db.Events.Find(eid).OrganizerID).ApiKey;
            string uri = "https://api.challonge.com/v1/tournaments/" + db.Tournaments.Find(tid).ApiId + "/matches.json?api_key=" + api_key;
            string matchData = SendRequest(uri);
            var matchObject = JToken.Parse(matchData);
            foreach (var m in matchObject)
            {
                Match newMatch = new Match
                {
                    TournamentID = tid ?? default(int),
                    Identifier = (string)m["match"]["identifier"],
                    Round = (int?)m["match"]["round"],
                    ApiID = (int)m["match"]["id"],
                    Competitor1ID = null,
                    Competitor2ID = null,
                    PrereqMatch1ID = null,
                    PrereqMatch2ID = null
                };
                if (m["match"]["player1_id"].ToString() != "")
                {
                    string temp = (string)participantsObject.Where(x => (int)x["participant"]["id"] == (int)m["match"]["player1_id"]).First()["participant"]["name"];
                    newMatch.Competitor1ID = db.Competitors.Where(x => x.EventID == eid).Where(x => x.CompetitorName == temp).First().CompetitorID;
                }
                if (m["match"]["player2_id"].ToString() != "")
                {
                    string temp = (string)participantsObject.Where(x => (int)x["participant"]["id"] == (int)m["match"]["player2_id"]).First()["participant"]["name"];
                    newMatch.Competitor2ID = db.Competitors.Where(x => x.EventID == eid).Where(x => x.CompetitorName == temp).First().CompetitorID;
                }
                if (m["match"]["player1_prereq_match_id"].ToString() != "")
                {
                    int prereqID = (int)m["match"]["player1_prereq_match_id"];
                    newMatch.PrereqMatch1ID = db.Matches.Where(x => x.TournamentID == tid).Where(x => x.ApiID == prereqID).First().MatchID;
                }
                if (m["match"]["player2_prereq_match_id"].ToString() != "")
                {
                    int prereqID = (int)m["match"]["player2_prereq_match_id"];
                    newMatch.PrereqMatch2ID = db.Matches.Where(x => x.TournamentID == tid).Where(x => x.ApiID == prereqID).First().MatchID;
                }
                db.Matches.Add(newMatch);
                db.SaveChanges();
            }
        }

        public JsonResult CompetitorList(int? id)
        {
            CompetitorUpdate(id);

            string compStr = "<table class=\"table table-bordered table-striped\"><tr><th>Competitors</th></tr>";
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

        public JsonResult MatchList(int? id)
        {
            string matchStr = "<h4 align=\"left\">Brackets</h4>";

            foreach(var t in db.Tournaments.Where(x =>x.EventID == id).ToList())
            {
                var matchList = db.Matches.Where(x => x.TournamentID == t.TournamentID).ToList();
                matchStr += "<div class =\"card\" style = \"background-color:lightgrey\"> <h5 align=\"left\">" + t.TournamentName + "</h5><div>";
                if (matchList.Any())
                {
                    var GFinal = (int)matchList.MaxBy(x => x.Round).First().Round;
                    var LFinal = (int)matchList.MinBy(x => x.Round).First().Round;
                    foreach (var m in matchList)
                    {
                        matchStr += "<table class=\"table table-bordered\" style=\"display: inline-block; border: solid; border-color:black; width:350px\">";
                        matchStr += "<tr style=\"height:30px\"><td width=\"20%\">";
                        matchStr += m.Identifier;
                        matchStr += "</td><td width=\"55%\">";

                        if (m.Competitor1ID != null)
                            matchStr += db.Competitors.Find(m.Competitor1ID).CompetitorName;
                        else
                        {
                            if ((m.Round > 0) || ((m.Round < 0) && (db.Matches.Find(m.PrereqMatch1ID).Round < 0)))
                                matchStr += "Winner of ";
                            else
                                matchStr += "Loser of ";
                            matchStr += db.Matches.Find(m.PrereqMatch1ID).Identifier;
                        }

                        matchStr += "</td><td width=\"25%\">";

                        if (m.Score1 == null)
                            matchStr += "<button id=\"start" + m.Identifier + "\" style=\"width: 100 % \">Start</button>";
                        else
                            matchStr += m.Score1;
                        matchStr += "</td></tr>";

                        matchStr += "<tr><td>";
                        if ((m.Round > 0) && (m.Round < (GFinal - 3)))
                            matchStr += "W" + m.Round;
                        else if ((m.Round < 0) && (m.Round > LFinal + 2))
                            matchStr += "L" + Math.Abs((int)m.Round);
                        else if (m.Round == GFinal)
                        {
                            if ((m.PrereqMatch1ID != null) && (db.Matches.Find(m.PrereqMatch1ID).Round == GFinal))
                                matchStr += "GFR";
                            else
                                matchStr += "GF";
                        }
                        else if (m.Round == GFinal - 1)
                            matchStr += "WF";
                        else if (m.Round == GFinal - 2)
                            matchStr += "WSF";
                        else if (m.Round == GFinal - 3)
                            matchStr += "WQF";
                        else if (m.Round == LFinal)
                            matchStr += "LF";
                        else if (m.Round == LFinal + 1)
                            matchStr += "LSF";
                        else if (m.Round == LFinal + 2)
                            matchStr += "LQF";
                        matchStr += "</td><td>";

                        if (m.Competitor2ID != null)
                            matchStr += db.Competitors.Find(m.Competitor2ID).CompetitorName;
                        else
                        {
                            if ((m.Round > 0) || ((m.Round < 0) && (db.Matches.Find(m.PrereqMatch2ID).Round < 0)))
                                matchStr += "Winner of ";
                            else
                                matchStr += "Loser of ";
                            matchStr += db.Matches.Find(m.PrereqMatch2ID).Identifier;
                        }

                        matchStr += "</td><td>";

                        if (m.Score2 == null)
                            matchStr += "<button id=\"submit" + m.Identifier + "\" style=\"width: 100 % \">Submit</button>";
                        else
                            matchStr += m.Score2;
                        matchStr += "</td></tr></table>";
                        matchStr += "<div style = \"display: inline-block; width: 5px\"></div>";
                    }
                    matchStr += "</div></div></br>";
                }
            }

            var data = new
            {
                matchTable = matchStr
            };

            return Json(data, JsonRequestBehavior.AllowGet);
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
    }
}
