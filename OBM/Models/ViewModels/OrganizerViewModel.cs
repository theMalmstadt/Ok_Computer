using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OBM.DAL;

namespace OBM.Models.ViewModels
{
    public class OrganizerViewModel
    {
        private EventContext db = new EventContext();

        public OrganizerViewModel(Organizer Org)
        {
            organizer = Org;
            var hasEvents = db.Events.Where(x => x.OrganizerID == Org.organizerID).Count() != 0;
            var temp = new List<Tuple<string, string, int, int>>();
            if (!hasEvents)
            {
                Tuple<string, string, int, int> eventNameLocationCompCountEventIDs = new Tuple<string, string, int, int>("No Events", "", 0, 0);
                temp.Add(eventNameLocationCompCountEventIDs);
            }
            else
            {
                foreach (var _event in db.Events.Where(x => x.OrganizerID == Org.organizerID).ToList())
                {
                    int count = _event.Competitors.Count();
                    var eventNameLocationCompCountEventIDs = new Tuple<string, string, int, int>(_event.EventName, _event.Location, count, _event.EventID);

                    temp.Add(eventNameLocationCompCountEventIDs);
                }
            }
            ListEventNameLocationCompCountEventIDs = temp;
        }
        public Organizer organizer { get; set; }

        public List<Tuple<string, string, int, int>> ListEventNameLocationCompCountEventIDs { get; set; }
    }
}