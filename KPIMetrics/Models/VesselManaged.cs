namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VesselManaged")]
    public partial class VesselManaged
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(14)]
        public string KPI_TYPE { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(7)]
        public string Movement { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(4)]
        public string Mgt_Type_Code { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime? Record_Date { get; set; }

        public int? countVsl { get; set; }
    }
}
