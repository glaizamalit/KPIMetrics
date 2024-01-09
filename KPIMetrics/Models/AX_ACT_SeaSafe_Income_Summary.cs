namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AX_ACT_SeaSafe_Income_Summary
    {
        [Key]
        [Column(Order = 0)]
        public int? TransYear { get; set; }

        [Key]
        [Column(Order = 1)]
        public string TransMonth { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(60)]
        public string AccountName { get; set; }     

        public int? ActualJobCount { get; set; }
    }
}
