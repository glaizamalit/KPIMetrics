namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPI_ControlPeriod
    {
        [Key]
        [Column(Order = 0)]
        public int ID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(9)]
        public string FinYear { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(3)]
        public string Model { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime StartDate { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime EndDate { get; set; }

        [Key]
        [Column(Order = 5)]
        public DateTime RecordStartDate { get; set; }

        [Key]
        [Column(Order = 6)]
        public DateTime RecordEndDate { get; set; }
    }
}
