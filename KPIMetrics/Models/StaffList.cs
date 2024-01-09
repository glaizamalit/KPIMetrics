namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StaffList")]
    public partial class StaffList
    {
        [StringLength(4)]
        public string Wallem_Initial { get; set; }

        [Key]
        [StringLength(6)]
        public string emp_no { get; set; }

        [StringLength(50)]
        public string DisplayName { get; set; }

        [StringLength(40)]
        public string FirstName { get; set; }

        [StringLength(20)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(30)]
        public string Dept_Desc { get; set; }

        [StringLength(128)]
        public string Division { get; set; }

        [StringLength(128)]
        public string BranchCity { get; set; }

        [StringLength(128)]
        public string Country { get; set; }

        [StringLength(80)]
        public string JobFamily { get; set; }

        [StringLength(80)]
        public string Emp_Entity { get; set; }

        [StringLength(128)]
        public string JobCode { get; set; }

        public DateTime? DateJoined { get; set; }

        public DateTime? DateExited { get; set; }

        [StringLength(50)]
        public string PrimaryEmail { get; set; }

        [StringLength(50)]
        public string StaffType { get; set; }

        [StringLength(128)]
        public string Division_Fin { get; set; }

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

        public DateTime? LastUpdate_Fin { get; set; }

        public bool? Headcount { get; set; }
        [StringLength(100)]
        public string HeadcountRemarks { get; set; }
    }
}
