using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace ClassProject.Models
{
    public class EventViewModel
    {
        private ResultContext db = new ResultContext();

        public EventViewModel(Event myEvent)
        {
           ResultList = db.Event_Results.Where(x => x.Event_Id == myEvent.Id).OrderBy(x=>x.RecordedTime).ToList();
            EventName = myEvent.EventName;
            EventId = myEvent.Id;

            Debug.WriteLine(ResultList.First().Athlete.AthleteName);
            
            
        }


        public IList<Event_Results> ResultList { get; private set; }

        public string EventName { get; private set; }

        public int EventId { get; private set; }
        

    }
}