using System;
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
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace OBM.Controllers
{
    public class CompetitorController : Controller
    {
        private EventContext db = new EventContext();

        // GET: Participant
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string AddChallongeCompetitor()
        {
            var myobject = new { api_key = Request.Params["api_key"], name = Request.Params["name"], url = Request.Params["myUrl"] };

            var myJson = JObject.FromObject(myobject);

            return ChallongePost(myJson).ToString();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateComp([Bind(Include ="CompetitorID,CompetitorName,EventID,BusyState,Event")] Competitor comp)
        //{

        //}

        public JObject ChallongePost(JObject myJSON)
        {
            myJSON.Add("private", myJSON["Private"]);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.challonge.com/v1/tournaments/"+ myJSON["api_key"] + "/participants.json");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Competitor resultCompetitor = new Competitor();


                    resultCompetitor.EventID = (int)myJSON["event_id"];
                    resultCompetitor.CompetitorName = (string)myJSON["name"];



                    //CreateComp(resultCompetitor);
                    return JObject.Parse(result);
                }//EXCEPTION HAnDlINGSGINJFDIGSF

            }
            catch (System.Net.WebException e)
            {
                return JObject.FromObject(new { error = "webException", message = "please ensure that you have not already create a tournament with this url, and that the URL, name, and event_id fields are accurate." });
            }

        }

        [HttpPost]
        public ActionResult AddSingleParticipant(string singleAdd, int ApiID, int? EventID)
        {
            var userApiKey = db.AspNetUsers.FindAsync(User.Identity.GetUserId()).Result.ApiKey;
            
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.challonge.com/v1/tournaments/" + ApiID + "/participants.json");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            JObject parameters = new JObject();
            parameters.Add("api_key", userApiKey);
            parameters.Add("name", singleAdd);

            using(var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(parameters);
            }
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
            }
            catch (System.Net.WebException e)
            {

            }
            return RedirectToAction("Tournament", "Events", new { id= EventID });
        }
    }
}