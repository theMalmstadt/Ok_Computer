namespace OBM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Competitor")]
    public partial class Competitor
    {
        //This section was creating the "Competitor_CompetitorID" error
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public Competitor()
        //{
        //    Matches = new HashSet<Match>();
        //}

        public int CompetitorID { get; set; }


        [StringLength(128)]
        public string CompetitorName { get; set; }

        [Required]
        public int EventID { get; set; }

        [StringLength(1)]
        public string BusyState { get; set; }

        public virtual Event Event { get; set; }

        [Display(Name = "Phone Number: ")]
        public string PhoneNumber { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Match> Matches { get; set; }
    }
}
