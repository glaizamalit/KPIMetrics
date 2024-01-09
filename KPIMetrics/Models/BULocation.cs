namespace KPIMetrics.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BULocation")]
    public partial class BULocation
    {
        [Key]
        [Column(Order = 0)]
        public int ID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BUID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(5)]
        public string Location { get; set; }
    }
}
