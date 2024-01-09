namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StaffHeadcount")]
    public partial class StaffHeadcount
    {
        [Key]
        [Column(Order = 0)]
        public int ID { get; set; }
       
        [StringLength(10)]
        public string Emp_No { get; set; }

        public int? VacancyID { get; set; }

        [StringLength(50)]
        public string Year { get; set; }

        [StringLength(50)]
        public string Month { get; set; }

        public bool? Headcount { get; set; }
        
        public bool IsDeleted { get; set; }
       
        [StringLength(50)]
        public string CreatedBy { get; set; }
       
        public DateTime CreatedDt { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDt { get; set; }
    }
}
