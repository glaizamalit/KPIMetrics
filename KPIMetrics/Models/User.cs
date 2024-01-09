namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string Initials { get; set; }

   
        public string Name { get; set; }      

   
        public bool IsActive { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime CreatedDt { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDt { get; set; }
    }
}
