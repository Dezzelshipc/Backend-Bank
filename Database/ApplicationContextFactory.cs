using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Database
{
    public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseNpgsql(
                $"Host=localhost;Port=5432;Database=backend_db2;Username=backend_user;Password=backend_pass");

            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}