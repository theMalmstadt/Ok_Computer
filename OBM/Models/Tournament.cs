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

        public int EventID { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        [StringLength(256)]
        public string Game { get; set; }

        public int? ApiId { get; set; }

        [Required]
        [StringLength(256)]
        public string UrlString { get; set; }

        public bool IsTeams { get; set; }
    }
}
