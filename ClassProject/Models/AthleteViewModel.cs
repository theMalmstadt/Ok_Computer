using System;
using System.Collections.Generic;
using System.Linq;
using ClassProject.Models;

namespace ClassProject.Models
{
    public class AthleteViewModel
    {
        private ResultContext db = new ResultContext();

        public AthleteViewModel(Athlete ath)
        {
            AthleteId = ath.Id;
            AthleteName = ath.AthleteName;
            var found = db.Event_Results.Where(x => x.Athlete_Id == ath.Id);
            AthleteResults = db.Event_Results.Where(x => x.Athlete_Id == ath.Id).ToList();
        }

        public int AthleteId { get; private set; }
        public string AthleteName { get; private set; }
        public IList<Event_Results> AthleteResults { get; private set; }
    }
}