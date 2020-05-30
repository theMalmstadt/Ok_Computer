using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBM.Models
{
    public partial class Organizer
    {
        [StringLength(128)]
        public string organizerName { get; set; }

        public string organizerID { get; set; }
    }
}