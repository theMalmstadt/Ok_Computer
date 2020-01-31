using System;
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
            AthleteResults = db.Event_Results.Where(x => x.Athlete_Id == ath.Id);
        }

        public int AthleteId { get; private set; }
        public string AthleteName { get; private set; }
        public IQueryable AthleteResults { get; private set; }
    }
}