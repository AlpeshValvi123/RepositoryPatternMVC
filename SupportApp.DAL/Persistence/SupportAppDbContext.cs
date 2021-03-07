using SupportApp.Core.Domain.User;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SupportApp.DAL.Persistence
{
    public class SupportAppDbContext : DbContext
    {
        public SupportAppDbContext() : base("ConnectionString")
        {
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public DbSet<User> Users { get; set; }
       
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}