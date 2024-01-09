namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPI_metric_log_detail
    {
        public int ID { get; set; }

        public int LogID { get; set; }

        [Required]
        [StringLength(100)]
        public string LogdFieldname { get; set; }

        [Required]
        public string LogdOldvalue { get; set; }

        [Required]
        public string LogdNewvalue { get; set; }
    }
}
