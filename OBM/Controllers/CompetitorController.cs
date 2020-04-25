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

        [HttpPost]
        public ActionResult AddSingleParticipant(string singleAdd, int ApiID, int? TourneyID)
        {
            if(!string.IsNullOrEmpty(singleAdd))
            {
                var userApiKey = db.AspNetUsers.FindAsync(User.Identity.GetUserId()).Result.ApiKey;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.challonge.com/v1/tournaments/" + ApiID + "/participants.json");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                JObject parameters = new JObject();
                parameters.Add("api_key", userApiKey);
                parameters.Add("name", singleAdd);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
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
                    TempData["Result"] = "Duplicate Participants not allowed.";
                    return RedirectToAction("Tournament", "Events", new { id = TourneyID });
                }

                TempData["Result"] = "Participant added!";

                return RedirectToAction("Tournament", "Events", new { id = TourneyID });
            }
            else
            {
                TempData["Result"] = "Must have at least one participant to add to tournament.";
                return RedirectToAction("Tournament", "Events", new { id = TourneyID });
            }
            
        }

        [HttpPost]
        public ActionResult BulkAddParticipants(string bulkadd, int ApiID, int TourneyID)
        {
            //CheckDBParticipants();
            if(!string.IsNullOrEmpty(bulkadd))
            {
                var userApiKey = db.AspNetUsers.FindAsync(User.Identity.GetUserId()).Result.ApiKey;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.challonge.com/v1/tournaments/" + ApiID + "/participants/bulk_add.json");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                char[] parser = { ' ', ',' };

                string[] bulkaddnames = bulkadd.Split(parser, StringSplitOptions.RemoveEmptyEntries);
                if (DuplicateParticipants(bulkaddnames))
                {
                    JObject senddata = new JObject();
                    senddata.Add("api_key", userApiKey);

                    JArray participants = new JArray();

                    foreach (var name in bulkaddnames)
                    {
                        JObject temp = new JObject();
                        temp.Add("name", name);

                        participants.Add(temp);
                    }

                    senddata.Add("participants", participants);

                    System.Diagnostics.Debug.WriteLine("\nJson: {\n" + participants + "\n}\n");

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(senddata);
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
                        TempData["Result"] = "Duplicate Participants not allowed.";
                        return RedirectToAction("Tournament", "Events", new { id = TourneyID });
                    }

                    TempData["Result"] = "Participants added!";

                    return RedirectToAction("Tournament", "Events", new { id = TourneyID });
                }
                else
                {
                    TempData["Result"] = "Must have at least one participant to add to tournament.";
                    return RedirectToAction("Tournament", "Events", new { id = TourneyID });
                }
            }
            ViewBag.bulkcannotbenull = true;
            return RedirectToAction("Tournament", "Events", new { id = TourneyID });
        }

        public static bool DuplicateParticipants(string[] participants)
        {
            //puts all elements into a hashset (doesn't make an entry if it matches another entry)
            HashSet<string> s = new HashSet<string>(participants);

            if(s.Count == participants.Length)
            {
                return (true);
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public JsonResult Seed(string json)
        {
            var newt = JObject.Parse(json);
            System.Diagnostics.Debug.WriteLine("\nJson: {\n" + json + "\n}\n");
            //System.Diagnostics.Debug.WriteLine("\nJson: {\n" + newt["id"] + "\n" + newt["ranks"] + "\n}\n");
            return Json("Success My Guy", JsonRequestBehavior.AllowGet);
        }
    }
}