namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPI_LifeBoats_JobDetail
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string JobNum { get; set; }

        [Required]
        [StringLength(100)]
        public string VslName { get; set; }

        [Required]
        [StringLength(10)]
        public string VslCode { get; set; }

        [Required]
        [StringLength(100)]
        public string Customer { get; set; }

        [Required]
        [StringLength(200)]
        public string CustomerCode { get; set; }

        [StringLength(20)]
        public string AcctCode { get; set; }

        [Required]
        [StringLength(50)]
        public string Model { get; set; }

        [Required]
        [StringLength(50)]
        public string AcctCodeDesc { get; set; }

        [Required]
        [StringLength(5)]
        public string PortCode { get; set; }

        [Required]
        [StringLength(100)]
        public string ContractType { get; set; }

        [Column("Inspection Type")]
        [Required]
        [StringLength(100)]
        public string Inspection_Type { get; set; }

        [Required]
        [StringLength(10)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }

        [StringLength(10)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDt { get; set; }
    }
}
