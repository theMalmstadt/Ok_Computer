namespace ClassProject.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ResultContext : DbContext
    {
        public ResultContext()
            : base("name=ClassProjectDB") // change this back if it does not work
        {
        }

        public virtual DbSet<Athlete> Athletes { get; set; }
        public virtual DbSet<Event_Results> Event_Results { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Race> Races { get; set; }
        public virtual DbSet<team_athlete> team_athlete { get; set; }
        public virtual DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Athlete>()
                .HasMany(e => e.Event_Results)
                .WithOptional(e => e.Athlete)
                .HasForeignKey(e => e.Athlete_Id);

            modelBuilder.Entity<Athlete>()
                .HasMany(e => e.team_athlete)
                .WithOptional(e => e.Athlete)
                .HasForeignKey(e => e.Athlete_Id);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Event_Results)
                .WithOptional(e => e.Event)
                .HasForeignKey(e => e.Event_Id);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.team_athlete)
                .WithOptional(e => e.Team)
                .HasForeignKey(e => e.Team_Id);
        }
    }
}
