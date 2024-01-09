namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPI_metric_log
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string LogTablename { get; set; }

        [Required]
        [StringLength(100)]
        public string LogActionType { get; set; }

        public DateTime LogDatetime { get; set; }

        public int LogTableID { get; set; }

        [Required]
        [StringLength(10)]
        public string LogUser { get; set; }

        [Required]
        [StringLength(100)]
        public string LogSource { get; set; }
    }
}
