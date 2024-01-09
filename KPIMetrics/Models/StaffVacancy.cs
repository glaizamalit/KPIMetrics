namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StaffVacancy")]
    public partial class StaffVacancy
    {
        [Key]
        [Column(Order = 0)]
        public int ID { get; set; }

        [StringLength(50)]
        public string Emp_No { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(30)]
        public string Dept_Desc { get; set; }

        [StringLength(128)]
        public string Division { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDt { get; set; }
    }
}
