using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace KPIMetrics.Models
{
    public partial class SQL6 : DbContext
    {
        public SQL6()
            : base("name=SQL6")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 3600; // seconds
        }

        //public virtual DbSet<KPI_Vessel_Management> KPI_Vessel_Management { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.KPI_TYPE)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.FYear)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.FQuarter)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.Key)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.IMO)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.Vessel_Code)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.Vessel_Name)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.MgtOfficeGroup)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.Mgt_Type_Code)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.MGT_Type_Desc)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.VslTypeGroup)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.Movement)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.In_Month)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.Out_Month)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.Mgt_Start_Date)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.Mgt_End_Date)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.OwnerGroup)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.OwnerNationality)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.ChangeReason)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.BASEAMOUNT)
        //        .HasPrecision(38, 16);

        //    modelBuilder.Entity<KPI_Vessel_Management>()
        //        .Property(e => e.Amount_HKD)
        //        .HasPrecision(38, 6);
        //}
    }
}
