using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OBM.DAL;

namespace OBM.Models.ViewModels
{
    public class ParticipantTournamentViewModel
    {
        private EventContext db = new EventContext();  

        public ParticipantTournamentViewModel(Tournament tourny, string competitors, string organizer)
        {
            tournyID = tourny.ApiId;
            participants = competitors;

            Event motherEvent = db.Events.Find(tourny.EventID);

            TournamentID = tourny.TournamentID;
            TournamentName = tourny.TournamentName;
            EventID = tourny.EventID;
            EventName = motherEvent.EventName;
            OrganizerID = motherEvent.EventName;
            OrganizerName = organizer;
            Description = tourny.Description;
            Game = tourny.Game;
            ApiId = tourny.ApiId;
            UrlString = tourny.UrlString;
            IsTeams = tourny.IsTeams ? "Yes" : "No";
            IsStarted = tourny.IsStarted ? "Yes" : "No";
            Public = motherEvent.Public;

        }
        public int? tournyID;
        public string participants;

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