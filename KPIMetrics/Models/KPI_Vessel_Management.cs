namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPI_Vessel_Management
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(14)]
        public string KPI_TYPE { get; set; }

        public int? CMonth { get; set; }

        [StringLength(7)]
        public string FYear { get; set; }

        [StringLength(10)]
        public string FQuarter { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Record_Date { get; set; }

        [StringLength(15)]
        public string Key { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string IMO { get; set; }

        [Column("Vessel Code")]
        [StringLength(5)]
        public string Vessel_Code { get; set; }

        [Key]
        [Column("Vessel Name", Order = 2)]
        [StringLength(100)]
        public string Vessel_Name { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(30)]
        public string MgtOfficeGroup { get; set; }

        [Column("Mgt Type Code")]
        [StringLength(4)]
        public string Mgt_Type_Code { get; set; }

        [Key]
        [Column("MGT Type Desc", Order = 4)]
        [StringLength(50)]
        public string MGT_Type_Desc { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(30)]
        public string VslTypeGroup { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(7)]
        public string Movement { get; set; }

        [StringLength(30)]
        public string In_Month { get; set; }

        [StringLength(30)]
        public string Out_Month { get; set; }

        [StringLength(30)]
        public string Mgt_Start_Date { get; set; }

        [StringLength(30)]
        public string Mgt_End_Date { get; set; }

        [StringLength(100)]
        public string OwnerGroup { get; set; }

        [StringLength(100)]
        public string OwnerNationality { get; set; }

        [StringLength(100)]
        public string ChangeReason { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? BASEAMOUNT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Amount_HKD { get; set; }
    }
}
