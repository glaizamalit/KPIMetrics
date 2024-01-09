using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace KPIMetrics.Models
{
    public partial class KPIMetrics : DbContext
    {
        public KPIMetrics()
            : base("name=KPIMetrics")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 3600; // seconds
        }

        public virtual DbSet<KPI_Commercial> KPI_Commercials { get; set; }
        public virtual DbSet<KPI_Metrics> KPI_Metrics { get; set; }
        public virtual DbSet<KPI_LifeBoats_Assignment> KPI_LifeBoats_Assignments { get; set; }
        public virtual DbSet<KPI_BU> KPI_BUs { get; set; }
        public virtual DbSet<KPI_LifeBoats_JobDetail> KPI_LifeBoats_JobDetails { get; set; }
        public virtual DbSet<KPI_metric_log> KPI_metric_logs { get; set; }
        public virtual DbSet<KPI_metric_log_detail> KPI_metric_log_details { get; set; }
        public virtual DbSet<KPI_References> KPI_References { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<KPI_ControlPeriod> KPI_ControlPeriods{ get; set; }
        public virtual DbSet<KPI_Agency> KPI_Agencies { get; set; }
        public virtual DbSet<KPI_Line_Function> KPI_Line_Functions { get; set; }
        public virtual DbSet<UserLocation> UserLocations { get; set; }
        public virtual DbSet<BULocation> BULocations { get; set; }
        public virtual DbSet<StaffList_Fin> StaffList_Fins { get; set; }
        public virtual DbSet<StaffList> StaffLists { get; set; }
        public virtual DbSet<KPI_Shipmanagement> KPI_Shipmanagements { get; set; }
        public virtual DbSet<KPI_Lifeboat> KPI_Lifeboats { get; set; }
        public virtual DbSet<StaffHeadcount> StaffHeadcounts { get; set; }
        public virtual DbSet<StaffVacancy> StaffVacancies { get; set; }
        public virtual DbSet<AX_ACT_SeaSafe_Income_Summary> AX_ACT_SeaSafe_Income_Summaries { get; set; }

        public virtual DbSet<VesselManaged> VesselManageds { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.BU)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.LOB)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.CalYear)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.CalMonth)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.FinYear)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.Model)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.KPI1)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.KPI2)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.KPI3)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.KPI4)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.Measure1)
                .HasPrecision(10, 4);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.Measure2)
                .HasPrecision(10, 4);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Commercial>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Metrics>()
                .Property(e => e.MetricName)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Metrics>()
                .Property(e => e.KPI1)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Metrics>()
                .Property(e => e.KPI2)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Metrics>()
                .Property(e => e.KPI3)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Metrics>()
                .Property(e => e.KPI4)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Metrics>()
                .Property(e => e.Measure1)
                .IsUnicode(false);

            modelBuilder.Entity<KPI_Metrics>()
                .Property(e => e.Measure2)
                .IsUnicode(false);
        }
    }
}
