namespace ClassProject.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class team_athlete
    {
        public int Id { get; set; }

        public int? Team_Id { get; set; }

        public int? Athlete_Id { get; set; }

        public virtual Athlete Athlete { get; set; }

        public virtual Team Team { get; set; }
    }
}
