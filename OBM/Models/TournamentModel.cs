namespace OBM.Models
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tournament")]
    public partial class Tournament
    {

        public int TournamentID { get; set; }

        [Required]
        [StringLength(128)]
        public string TournamentName { get; set; }

        [Required]
        public int EventID { get; set; }

        public String Game { get; set; }
        public String ApiId  { get; set; }

        public String UrlString { get; set; }


        public Boolean isTeams { get; set; }


    }
}