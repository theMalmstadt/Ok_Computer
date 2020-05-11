using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using OBM.DAL;
using OBM.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using reCAPTCHA.MVC;
using System.Configuration;
using System.Text.RegularExpressions;

//For SMS Contacting
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

using Twilio.TwiML;
using Twilio.AspNet.Mvc;
using Twilio.Exceptions;

namespace OBM.Controllers
{
    public class CompetitorController : Controller
    {
        private EventContext db = new EventContext();

        [HttpPost]
        public ActionResult AddSingleParticipant(string singleAdd, int? TourneyID)
        {
            var tourney = db.Tournaments.Find(TourneyID);
            if (!string.IsNullOrEmpty(singleAdd))
            {
                var userApiKey = db.AspNetUsers.FindAsync(User.Identity.GetUserId()).Result.ApiKey;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.challonge.com/v1/tournaments/" + tourney.ApiId + "/participants.json");
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
        public ActionResult BulkAddParticipants(string bulkAdd, int TourneyID)
        {
            var tourney = db.Tournaments.Find(TourneyID);
            //CheckDBParticipants();
            if (!string.IsNullOrEmpty(bulkAdd))
            {
                var userApiKey = db.AspNetUsers.FindAsync(User.Identity.GetUserId()).Result.ApiKey;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.challonge.com/v1/tournaments/" + tourney.ApiId + "/participants/bulk_add.json");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                char[] parser = { ' ', ',' };

                string[] bulkaddnames = bulkAdd.Split(parser, StringSplitOptions.RemoveEmptyEntries);
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

                //var seededCompetitors = rankedCompetitors;
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

                var sendingJson = "{\"participants\":[";
                var count = 1;
                foreach (var comp in seededCompetitors)
                {
                    sendingJson += "{\"name\":\"" + comp + "\",\"seed\":" + count + "},";
                    count++;
                }
                sendingJson = sendingJson.Remove(sendingJson.Length - 1);
                sendingJson += "]}";

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
            //var stringToSend = "{\"participants\":[{\"name\":\"comp1\",\"seed\":1},{\"name\":\"comp2\",\"seed\":2},{\"name\":\"comp3\",\"seed\":3},{\"name\":\"comp4\",\"seed\":4}]}";

            Debug.WriteLine(json);

            // First API call here
            // PUT request to clear all participants from Challonge Tournament
            ClearCompetitors(tourn.TournamentID);

            // Second API call here
            // POST the json string above


            string challongeResponse = BulkAddCompetitorsSeeds(json, tourn);
            
            return challongeResponse;
        }
        public void ClearCompetitors(int? TourneyId)
        {
            var tourney = db.Tournaments.Find(TourneyId);
            //DELETE Help https://api.challonge.com/v1/tournaments/{tournament}/participants/clear.{json|xml}
            string userId = (HttpContext.User.Identity.GetUserId());
            string apiKey = db.AspNetUsers.Find(userId).ApiKey;
            string apikeyString = $"api_key={apiKey}";
            string url = $"https://api.challonge.com/v1/tournaments/{tourney.ApiId}/participants/clear.json?{apikeyString}";
            
            //CONFIGURE REQUEST
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "DELETE";
     
            //SEND REQUEST

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Debug.WriteLine(httpResponse);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Debug.WriteLine(result);
                }
            }
            catch (System.Net.WebException e)
            {
                Debug.WriteLine("There was a problem deleting particpants from the tournament to seed");
            }

                    
        }

        public ActionResult DeleteCompetitors(int TourneyID, List<string> CompNames = null, bool clearAll = false)
        {
            var tourney = db.Tournaments.Find(TourneyID);
            var TourneyComps = GetCompetitors(tourney.UrlString);
            if (CompNames != null)
            {
                if ((TourneyComps.Count == CompNames.Count) || clearAll)
                {
                    ClearCompetitors(tourney.TournamentID);
                }
                else
                {
                    //CompsToRemove["CompName"]["ChallongeUID"]
                    var compsToRemove = ParseMatchingCompetitiorsAndUID(ParseCompNameAndId(TourneyComps), CompNames);

                    foreach (var item in compsToRemove)
                    {
                        SingleDeleteCompetitors(item.Item2, tourney.ApiId);
                    }
                }
            }
            else if (clearAll == true)
            {
                ClearCompetitors(tourney.TournamentID);
            }

            return RedirectToAction("Tournament", "Events", new { id = TourneyID });
        }

        public void SingleDeleteCompetitors(string CompChallongeUID, int? ApiId)
        {
            //DELETE Help https://api.challonge.com/v1/tournaments/{tournament}/participants/clear.{json|xml}
            string userId = (HttpContext.User.Identity.GetUserId());
            string apiKey = db.AspNetUsers.Find(userId).ApiKey;
            string apikeyString = $"api_key={apiKey}";
            string url = $"https://api.challonge.com/v1/tournaments/{ApiId}/participants/{CompChallongeUID}.json?{apikeyString}";

            //CONFIGURE REQUEST
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "DELETE";

            //SEND REQUEST

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
                Debug.WriteLine("There was a problem deleting particpants from the tournament to seed");
            }
        }

        public JArray GetCompetitors(string TourneyUrl) 
        {
            //GET all Competitors
            string userId = (HttpContext.User.Identity.GetUserId());
            string apiKey = db.AspNetUsers.Find(userId).ApiKey;
            string apikeyString = $"api_key={apiKey}";
            string url = $"https://api.challonge.com/v1/tournaments/{TourneyUrl}/participants.json?{apikeyString}";

            var participants = new JArray();
            //Form the Request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    participants = JArray.Parse(result);
                }
                return participants;
            }
            catch (System.Net.WebException e)
            {
                Debug.WriteLine("There was a problem deleting particpants from the tournament to seed");
                return participants;
            }
        }

        public List<Tuple<string, string>> ParseCompNameAndId(JArray competitors)
        {
            var competitorAndId = new List<Tuple<string, string>>();
            foreach (var item in competitors)
            {
                var compName = $"{item["participant"]["name"]}";
                var compUID = $"{item["participant"]["id"]}";
                var nameUID = new Tuple<string, string>(compName, compUID);
                competitorAndId.Add(nameUID);
            }
            return competitorAndId;
        }

        public List<Tuple<string, string>> ParseMatchingCompetitiorsAndUID(List<Tuple<string, string>> bulkComps, List<string> matchingComps)
        {
            var Competitors = new List<Tuple<string, string>>();
            foreach (var bulk in bulkComps)
            { 
                foreach(var matching in matchingComps)
                {
                    if(bulk.Item1 == matching)
                    {
                        Competitors.Add(bulk);
                    }
                }
            }

            return Competitors;
        }

        public string BulkAddCompetitorsSeeds(string json, Tournament tourn)
        {
            string userId = (HttpContext.User.Identity.GetUserId());
            string apiKey = db.AspNetUsers.Find(userId).ApiKey;
            string apikeyString = $"api_key={apiKey}";

            // POST https://api.challonge.com/v1/tournaments/{tournament}/participants/bulk_add.{json|xml}
            string url = "https://api.challonge.com/v1/tournaments/" + tourn.ApiId + $"/participants/bulk_add.json?{apikeyString}";
            //Debug.WriteLine(json);


            //CONFIGURE REQUEST
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";


            //WRITE DATA TO REQUEST BODY
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
            }
            //SEND REQUEST
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Debug.WriteLine(result);
                    return result;
                }
            }
            catch(System.Net.WebException e)
            {
                Debug.WriteLine("problem posting competitors with seeds to challonge");
                return "seed posting failed :(";
            }
        }

        [HttpGet]
        public ActionResult UpdateContact(int id)
        {
            Competitor model = db.Competitors.Find(id);
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaValidator]
        public ActionResult UpdateContact(Competitor competitor)
        {
            var dbCompetitior = db.Competitors.FirstOrDefault(c => c.CompetitorID.Equals(competitor.CompetitorID));

            dbCompetitior.PhoneNumber = competitor.PhoneNumber;
            dbCompetitior.CompetitorName = competitor.CompetitorName;

            db.SaveChanges();

            return RedirectToAction("Manage", "Events", new { id = competitor.EventID });
        }

        /// <summary>
        /// This feature works, but not at full potential. Currently uses a Test account to send text messages and will not actually send a text to
        /// the competitor without a registerd Twilio Phone Number. If you want this feature to work you need to provide an actual Twilio Accound Sid 
        /// and Authentication Token.
        /// </summary>
        /// <param name="CompID">ID of the competitor to find the competitor's provided contact number</param>
        /// <param name="EventID">ID of the event the competitor is in to make the text message personalized to the competitor, to obtain Event Organizer's name.</param>
        /// <returns></returns>
        public JsonResult SMSContact(int CompID, int EventID)
        {
            var dbComp = db.Competitors.Find(CompID);
            var dbEvent = db.Events.Find(EventID);
            var dbEventOrganizer = db.AspNetUsers.Find(dbEvent.OrganizerID);

            var accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
            var authoToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
            TwilioClient.Init(accountSid, authoToken);

            string compNumTrimmed = Regex.Replace(dbComp.PhoneNumber, @"[^\d]", "");

            var to = new PhoneNumber("+1" + compNumTrimmed);
            var from = new PhoneNumber(ConfigurationManager.AppSettings["TwilioPhoneNumber"]);
            try
            {
                var message = MessageResource.Create(
                to: to,
                from: from,
                body: $"Your match in: {dbEvent.EventName} is about to start, report to Event Organizer: {dbEventOrganizer.UserName}, to begin your match."
                );

                List<object> data = new List<object>();

                var obj = new
                {
                    statusMSG = "Competitor Contacted",
                    contactName = dbComp.CompetitorName
                };

                data.Add(obj);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch(TwilioException e)
            {
                List<object> data = new List<object>();
                var obj = new
                {
                    statusMSG = string.Format($"{e.Message}"),
                    contactName = dbComp.CompetitorName
                };

                data.Add(obj);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}