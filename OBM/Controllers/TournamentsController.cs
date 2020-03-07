﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OBM.DAL;
using OBM.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace OBM.Controllers
{

    public class TournamentsController : Controller
    {
        private static readonly HttpClient client = new HttpClient();

        private EventContext db = new EventContext();
        private ApplicationDbContext userdb = new ApplicationDbContext();

        // GET: Tournaments
        public ActionResult Index()
        {
            return View(db.Tournaments.ToList());
        }

        // GET: Tournaments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // GET: Tournaments/Create
        public ActionResult Create()
        {           //FETCH NEEDED USER DATA

            var userid = HttpContext.User.Identity.GetUserId();
            var UserEventsList = db.Events.Where(x=>x.OrganizerID== userid);
            ViewBag.UserEventsList = UserEventsList;

            var userApiKey = db.AspNetUsers.Where(x => x.Id == userid).First().ApiKey;
            Debug.WriteLine(userApiKey);
            ViewBag.ApiKey = userApiKey;
            


            return View();
        }

        // POST: Tournaments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TournamentID,TournamentName,EventID,Description,Game,ApiId,UrlString,IsTeams")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                db.Tournaments.Add(tournament);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tournament);
        }

        // GET: Tournaments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // POST: Tournaments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TournamentID,TournamentName,EventID,Description,Game,ApiId,UrlString,IsTeams")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tournament).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tournament);
        }

        // GET: Tournaments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // POST: Tournaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tournament tournament = db.Tournaments.Find(id);
            db.Tournaments.Remove(tournament);
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


        [HttpPost]
        public  string ChallongeCreate()
        {
            //PARSE VALUES
            var myobject = new { api_key = Request.Params["api_key"], name = Request.Params["name"], description = Request.Params["description"], game = Request.Params["game"], url = Request.Params["myURL"], event_id = Request.Params["event_id"], Private = Request.Params["private"] };

           var myJSON= JObject.FromObject(myobject);

           
           
            return ChallongePost(myJSON).ToString();

        }



        public   JObject ChallongePost(JObject myJSON)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.challonge.com/v1/tournaments.json");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                

                streamWriter.Write(myJSON);
            }
            Debug.WriteLine(myJSON.ToString());
            Debug.WriteLine("api_key is : " + myJSON["api_key"]);
            Debug.WriteLine("url is : " + myJSON["url"]);
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Debug.WriteLine(result);

                return JObject.Parse(result);
            }//EXCEPTION HAnDlINGSGINJFDIGSF
            
        }





    }
}