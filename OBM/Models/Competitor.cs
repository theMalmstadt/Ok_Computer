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

        public int CompetitorID { get; set; }
        
        [Required]
        [StringLength(128)]
        public string CompetitorName { get; set; }

        [Required]
        public int EventID { get; set; }

        [StringLength(1)]
        public string BusyState { get; set; }
        

    }
}