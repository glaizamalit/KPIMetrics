namespace KPIMetrics.Models
{
    using System.Data.Entity;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class VRMdbModelsDbContext : DbContext
    {
        public VRMdbModelsDbContext()
            : base("name=VRMdbModels")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }

    }

    public class IsGroupMemberResult
    {
        public string initial { get; set; }
    }
}