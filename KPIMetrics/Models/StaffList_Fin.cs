namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StaffList_Fin
    {
        [Key]
        [StringLength(10)]
        public string Emp_No { get; set; }

        [StringLength(50)]
        public string StaffType { get; set; }

        [StringLength(128)]
        public string BU_Fin { get; set; }

        [StringLength(128)]
        public string Func_Fin { get; set; }

        [StringLength(128)]
        public string Job_Fin { get; set; }

        [StringLength(50)]
        public string AXcode { get; set; }

        [StringLength(128)]
        public string Entity_Fin { get; set; }

        [StringLength(128)]
        public string Division_Fin { get; set; }

        public bool? Headcount { get; set; }

        [StringLength(100)]
        public string HeadcountRemarks { get; set; }

        [StringLength(10)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDT { get; set; }

        [Required]
        [StringLength(10)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDT { get; set; }
    }
}
