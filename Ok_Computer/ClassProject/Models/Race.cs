namespace ClassProject.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Race
    {
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string EventName { get; set; }
    }
}
