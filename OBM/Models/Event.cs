namespace OBM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event
    {
        public int EventID { get; set; }

        [Required]
        [StringLength(128)]
        public string OrganizerID { get; set; }

        [Required]
        [StringLength(128)]
        public string EventName { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        [StringLength(256)]
        public string Location { get; set; }

        public bool Public { get; set; }
    }
}
