using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OBM.DAL;

namespace OBM.Models.ViewModels
{
    public class MatchViewModel
    {
        private EventContext db = new EventContext();

        public MatchViewModel(Match match)
        {
            MatchID = match.MatchID;
            TournamentID = match.TournamentID;
            Competitor1ID = match.Competitor1ID;
            Competitor1Name = db.Competitors.Find(match.Competitor1ID).CompetitorName;
            Competitor2ID = match.Competitor2ID;
            Competitor2Name = db.Competitors.Find(match.Competitor2ID).CompetitorName;
            Score1 = match.Score1;
            Score2 = match.Score2;
            Identifier = match.Identifier;
            Round = match.Round;
            ApiID = match.ApiID;
            PrereqMatch1ID = match.PrereqMatch1ID;
            PrereqMatch2ID = match.PrereqMatch2ID;
            Time = match.Time.ToString();
            Winner = (Score1 > Score2) ? Competitor1ID : Competitor2ID;
    
            if ((match.Score1 == null) || (match.Score2 == null))
            {
                if ((DateTime.Compare(match.Time, DateTime.Now) > 0) || (match.Time == null))
                {
                    Status = "upcoming";
                }
                else
                {
                    Status = "in progress";
                }
            }
            else
            {
                Status = "completed";
            }
        }

        public int MatchID { get; set; }
        public int TournamentID { get;  set; }
        public int? Competitor1ID { get; set; }
        public string Competitor1Name { get; set; }
        public int? Competitor2ID { get; set; }
        public string Competitor2Name { get; set; }
        public int? Score1 { get; set; }
        public int? Score2 { get; set; }
        public string Identifier { get; set; }
        public int? Round { get; set; }
        public int ApiID { get; set; }
        public int? PrereqMatch1ID { get; set; }
        public int? PrereqMatch2ID { get; set; }
        public string Time { get; set; }
        public int? Winner { get; set; }
        public string Status { get; set; }
    }
}