using OBM.Models;

namespace OBM.DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EventContext : DbContext
    {
        public EventContext()
            : base("name=OBMDB_Azure")
        {
        }

        public virtual DbSet<Event> Events { get; set; }

        public virtual DbSet<Competitor> Competitors { get; set; }

        public virtual DbSet<Tournament> Tournaments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
