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
using System.Web.Helpers;

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
            var seedObject = JObject.Parse(json);
            /*
                json = {
                    id: id,                 <-- int
                    method: method,         <-- int
                    ranks: rankList,        <-- list of strings
                    groups: allGroups,      <-- list of list of strings
                    competitors: allComp    <-- list of strings
                };
            System.Diagnostics.Debug.WriteLine("\nJson: {\n" + json + "\n}\n");
            */

            Tournament found = db.Tournaments.Find((int)seedObject["id"]);
            if (db.Events.Find(found.EventID).OrganizerID == User.Identity.GetUserId())
            {
                var rankedCompetitors = new List<string>();
                foreach (var rank in seedObject["ranks"])
                {
                    rankedCompetitors.Add(rank.ToString());
                }

                var leftovers = new List<string>();
                foreach (var comp in seedObject["competitors"])
                {
                    leftovers.Add(comp.ToString());
                }
                leftovers = leftovers.Except(rankedCompetitors).ToList();

                var filteredGroups = new List<List<string>>();
                foreach (var group in seedObject["groups"])
                {
                    if (group.Any())
                    {
                        var oldGroup = group.ToList();
                        var newGroup = new List<string>();
                        foreach (var element in oldGroup)
                        {
                            newGroup.Add(element.ToString());
                        }
                        newGroup = newGroup.Except(rankedCompetitors).ToList();
                        filteredGroups.Add(newGroup);
                        leftovers = leftovers.Except(newGroup).ToList();
                    }
                }

                var seededCompetitors = new List<string>();

                if (seedObject["method"].ToString() == "seq")
                {
                    seededCompetitors.AddRange(rankedCompetitors);
                }
                else
                {
                    filteredGroups.Add(rankedCompetitors);
                }

                foreach (var group in filteredGroups)
                {
                    int spacing = (int)Math.Floor((double)leftovers.Count() / group.Count()) + 1;
                    for (var i = 0; i < group.Count(); i++)
                    {
                        leftovers.Insert((i * spacing), group[i]);
                    }
                }
                seededCompetitors.AddRange(leftovers);

                var sendingJson = "[";
                var count = 1;
                foreach (var comp in seededCompetitors)
                {
                    sendingJson += "{'participant':{'name':'" + comp + "','seed':" + count + "}},";
                    count++;
                }
                sendingJson = sendingJson.Remove(sendingJson.Length - 1);
                sendingJson += "]";

                var response = sendSeed(sendingJson, found);

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Access Denied", JsonRequestBehavior.AllowGet);
            }
        }

        public string sendSeed(string json, Tournament tourn)
        {
            // Get this string to be accepted by Challonge first.
            // Once this string works, use the json argument above
            var stringToSend = "[{'participant':{'name':'comp1','seed':1}},{'participant':{'name':'comp2','seed':2}},{'participant':{'name':'comp3','seed':3}},{'participant':{'name':'comp4','seed':4}}]";


            // First API call here
            // PUT request to clear all participants from Challonge Tournament


            // Second API call here
            // POST the json string above


            string challongeResponse = "success"; //This will be the json response you get from Challonge's API
            return challongeResponse;
        }

        
    }
}