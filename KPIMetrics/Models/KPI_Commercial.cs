namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPI_Commercial
    {
        public int ID { get; set; }

        public int SetID { get; set; }
        public int MetricID { get; set; }

        [Required]
        [StringLength(50)]
        public string BU { get; set; }

        [Required]
        [StringLength(50)]
        public string LOB { get; set; }

        [Required]
        [StringLength(50)]
        public string CalYear { get; set; }

        [Required]
        [StringLength(50)]
        public string CalMonth { get; set; }

        [StringLength(8)]
        public string FinYear { get; set; }

        [Required]
        [StringLength(50)]
        public string Model { get; set; }

        [StringLength(100)]
        public string KPI1 { get; set; }

        [StringLength(100)]
        public string KPI2 { get; set; }

        [StringLength(100)]
        public string KPI3 { get; set; }

        [StringLength(100)]
        public string KPI4 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Measure1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Measure2 { get; set; }

        [Required]
        [StringLength(10)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }

        [StringLength(10)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDt { get; set; }

        [Required]
        [StringLength(3)]
        public string Cur { get; set; }
    }
}
