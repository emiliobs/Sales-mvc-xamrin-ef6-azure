namespace Sales.Domain.Models
{
    using System.Data.Entity;
    public class DataContext : DbContext
    {
        #region Constructs
        public DataContext() : base("DefaultConnection")
        {

        }

        #endregion

        #region Properties
        public DbSet<Common.Models.Product> Products { get; set; }
        public System.Data.Entity.DbSet<Sales.Common.Models.Category> Categories { get; set; }
        #endregion


    }
}
