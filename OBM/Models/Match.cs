namespace OBM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Match")]
    public partial class Match
    {
        public int MatchID { get; set; }

        public int TournamentID { get; set; }

        public int? Competitor1ID { get; set; }

        public int? Competitor2ID { get; set; }

        public int? Score1 { get; set; }

        public int? Score2 { get; set; }

        [StringLength(16)]
        public string Identifier { get; set; }

        public int? Round { get; set; }

        public int ApiID { get; set; }

        public int? PrereqMatch1ID { get; set; }

        public int? PrereqMatch2ID { get; set; }

        public DateTime? Time { get; set; }
    }
}
