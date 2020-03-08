using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using OBM.DAL;
using OBM.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Host.SystemWeb;

namespace OBM.Models.ViewModels
{

    public partial class TournamentViewModel
    {
        private EventContext db = new EventContext();

        public TournamentViewModel(Tournament tour, string org)
        {
            Event motherEvent = db.Events.Find(tour.EventID);

            TournamentID = tour.TournamentID;
            TournamentName = tour.TournamentName;
            EventID = tour.EventID;
            EventName = motherEvent.EventName;
            OrganizerID = motherEvent.EventName;
            OrganizerName = org;
            Description = tour.Description;
            Game = tour.Game;
            ApiId = tour.ApiId;
            UrlString = tour.UrlString;
            IsTeams = tour.IsTeams ? "Yes" : "No";
            IsStarted = tour.IsStarted ? "Yes" : "No";
            Public = motherEvent.Public;
        }

        public int TournamentID { get; set; }
        public string TournamentName { get; set; }
        public int EventID { get; set; }
        public object EventName { get; set; }
        public string OrganizerID { get; set; }
        public string OrganizerName { get; set; }
        public string Description { get; set; }
        public string Game { get; set; }
        public int? ApiId { get; set; }
        public string UrlString { get; set; }
        public string IsTeams { get; set; }
        public string IsStarted { get; set; }
        public bool Public { get; set; }
    }
}