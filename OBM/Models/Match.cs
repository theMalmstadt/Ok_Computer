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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Match()
        {
            Match1 = new HashSet<Match>();
            Match11 = new HashSet<Match>();
        }

        public int MatchID { get; set; }

        public int TournamentID { get; set; }

        public int? Competitor1ID { get; set; }

        public int? Competitor2ID { get; set; }

        [StringLength(16)]
        public string Identifier { get; set; }

        public int? Round { get; set; }

        public int ApiID { get; set; }

        public int? PrereqMatch1ID { get; set; }

        public int? PrereqMatch2ID { get; set; }

        public DateTime? Time { get; set; }

        public virtual Competitor Competitor { get; set; }

        public virtual Competitor Competitor1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Match> Match1 { get; set; }

        public virtual Match Match2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Match> Match11 { get; set; }

        public virtual Match Match3 { get; set; }

        public virtual Tournament Tournament { get; set; }
    }
}
