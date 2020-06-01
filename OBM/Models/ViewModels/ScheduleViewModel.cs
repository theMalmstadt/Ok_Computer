using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OBM.DAL;

namespace OBM.Models.ViewModels
{
    public class ScheduleViewModel
    {
        private EventContext db = new EventContext();

        public ScheduleViewModel(Match match, int startTime, int interval, int station, int percentage)
        {
            MatchID = match.MatchID;
            TournamentID = match.TournamentID;
            TournamentName = db.Tournaments.Find(match.TournamentID).TournamentName;
            if (match.Competitor1ID != null)
            {
                Competitor1Name = db.Competitors.Find(match.Competitor1ID).CompetitorName;
            }
            else
            {
                Competitor1Name = "winner of " + db.Matches.Find(match.PrereqMatch1ID).Identifier;
            }
            if (match.Competitor2ID != null)
            {
                Competitor2Name = db.Competitors.Find(match.Competitor2ID).CompetitorName;
            }
            else
            {
                Competitor2Name = "winner of " + db.Matches.Find(match.PrereqMatch2ID).Identifier;
            }
            Round = match.Round;
            Identifier = match.Identifier;
            StartTime = startTime;
            MatchInterval = interval;
            Station = station;
            Percentage = percentage;
        }

        public int MatchID { get; set; }
        public int TournamentID { get; set; }
        public string TournamentName { get; set; }
        public string Competitor1Name { get; set; }
        public string Competitor2Name { get; set; }
        public int? Round { get; set; }
        public string Identifier { get; set; }
        public int StartTime { get; set; }
        public int MatchInterval { get; set; }
        public int Station { get; set; }
        public int Percentage { get; set; }
    }
}