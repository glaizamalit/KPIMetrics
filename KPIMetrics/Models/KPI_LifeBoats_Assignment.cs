namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPI_LifeBoats_Assignment
    {
        public int ID { get; set; }

        [Required]
        [StringLength(10)]
        public string WallemInit { get; set; }

        [StringLength(100)]
        public string StaffName { get; set; }

        [Required]
        [StringLength(50)]
        public string CalYear { get; set; }

        [Required]
        [StringLength(50)]
        public string CalMonth { get; set; }

        [Required]
        [StringLength(50)]
        public string CalDate { get; set; }

        [StringLength(8)]
        public string FinYear { get; set; }

        [StringLength(20)]
        public string JobNum { get; set; }

        [Required]
        [StringLength(10)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }

        [StringLength(10)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDt { get; set; }
    }
}
