namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPI_Metrics
    {
        [Key]
        [Column(Order = 0)]
        public int ID { get; set; }

        public int BUID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string MetricName { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string KPI1 { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(100)]
        public string KPI2 { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(100)]
        public string KPI3 { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(100)]
        public string KPI4 { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(100)]
        public string Measure1 { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(100)]
        public string Measure2 { get; set; }
    }
}
