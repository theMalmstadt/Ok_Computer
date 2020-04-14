using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using OBM.DAL;
using OBM.Models;
using OBM.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Host.SystemWeb;

namespace OBM.Models.ViewModels
{

    public partial class EventViewModel
    {
        private readonly Event db = new Event();

        public EventViewModel(Event @event, string orgName)
        {
            EventID = @event.EventID;
            OrganizerID = @event.OrganizerID;
            OrganizerName = orgName;
            EventName = @event.EventName;
            Description = @event.Description;
            Location = @event.Location;
            Public = @event.Public;
        }

        public int EventID { get; set; }

        public string OrganizerID { get; set; }

        public string OrganizerName { get; set; }

        public string EventName { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public bool Public { get; set; }
    }
}
