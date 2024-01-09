namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPI_References
    {
        [Key]
        [Column(Order = 0)]
        public int ID { get; set; }

        [Key]
        [Column(Order = 1)]
        public int BUID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string RefType { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(200)]
        public string RefValue { get; set; }
    }
}
