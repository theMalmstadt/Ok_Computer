namespace OBM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Station")]
    public partial class Station
    {
        public int StationID { get; set; }

        [Required]
        [StringLength(128)]
        public string StationName { get; set; }

        [StringLength(256)]
        public string Description { get; set; }
    }
}
