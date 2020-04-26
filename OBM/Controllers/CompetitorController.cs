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
            System.Diagnostics.Debug.WriteLine("\nJson: {\n" + json + "\n}\n");

            var seedObject = JObject.Parse(json);
            //System.Diagnostics.Debug.WriteLine("\nJson: {\n" + seedObject["competitors"].Count() + "\n}\n");
            //System.Diagnostics.Debug.WriteLine("\nJson: {\n" + newt["id"] + "\n" + newt["ranks"][0] + "\n" + newt["groups"][0][0] + "\n}\n");

            /*
                var data = {
                    id: id,
                    method: method,
                    ranks: rankList,
                    groups: allGroups,
                    competitors: allComp
                };
            */
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
                    //System.Diagnostics.Debug.WriteLine("\nJson: {\n" + filteredGroups[i][0].ToString() + "\n}\n");
                }
            }

            var seededCompetitors = new List<string>();

            if (seedObject["method"].ToString() == "seq")
            {
                foreach (var group in filteredGroups)
                {
                    int spacing = (int)Math.Floor((double)leftovers.Count() / group.Count()) + 1;
                    for(var i = 0; i < group.Count(); i++ )
                    {
                        leftovers.Insert((i*spacing), group[i]);
                    }
                }
                seededCompetitors.AddRange(rankedCompetitors);
                seededCompetitors.AddRange(leftovers);
            }

            for (var i = 0; i < seededCompetitors.Count(); i++)
            {

                System.Diagnostics.Debug.WriteLine(seededCompetitors[i]);
            }

            /*for (var i = 0; i < allCompetitors.Count(); i++)
            {

                System.Diagnostics.Debug.WriteLine(allCompetitors[i]);
            }*/
            /*
            var seededCompetitors = new List<string>();
            if (seedObject["method"].ToString() == "seq")
            {
                //System.Diagnostics.Debug.WriteLine("\nJson: {\n" + "seq confirmed" + "\n}\n");
                for (var i = 0; i < seedObject["ranks"].Count(); i ++)
                {
                    seededCompetitors.Add(seedObject["ranks"][i].ToString());
                }

                //for (var i = 0; i < seedObject["groups"].Count)

                for (var i = 0; i < seedObject["competitors"].Count(); i++)
                {
                    var temp = seedObject["competitors"][i].ToString();
                    if (!seededCompetitors.Contains(temp))
                    {
                        seededCompetitors.Add(temp);
                    }
                }

            }

            for (var i = 0; i < seededCompetitors.Count(); i++)
            {

                System.Diagnostics.Debug.WriteLine(seededCompetitors[i].ToString());
            }*/


            /*var filteredGroups = new List<List<string>>();
            if (seedObject["groups"].Any())
            {
                for (var i = 0; i < seedObject["groups"].Count(); i++)
                {
                    if (seedObject["groups"][i].Any())
                    {
                        var oldGroup = seedObject["groups"][i].ToList();
                        var newGroup = new List<string>();
                        for (var j = 0; j < oldGroup.Count(); j++ )
                        {
                            newGroup.Add(oldGroup[j].ToString());
                        }
                        filteredGroups.Add(newGroup);
                        //System.Diagnostics.Debug.WriteLine("\nJson: {\n" + filteredGroups[i][0].ToString() + "\n}\n");
                    }
                }
            }*/

            if (seedObject["ranks"].Count() == seedObject["competitors"].Count())
            {
                //System.Diagnostics.Debug.WriteLine("\nJson: {\n" + "full" + "\n}\n");
            }
            else
            {

            }
            

            /*
            var stringToSend = "[{'participant':{'name':'comp1','seed':1}},{'participant':{'name':'comp2','seed':2}},{'participant':{'name':'comp3','seed':3}},{'participant':{'name':'comp4','seed':4}}]";
            dynamic data = System.Web.Helpers.Json.Decode(stringToSend);
            string trying = data[0]["participant"]["name"].ToString();
            System.Diagnostics.Debug.WriteLine("\nJson: {\n" + trying + "\n}\n");
            var finish = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine("\nJson: {\n" + finish + "\n}\n");
            */

            return Json("Success My Guy", JsonRequestBehavior.AllowGet);
        }
    }
}