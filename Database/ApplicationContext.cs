using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<TokenModel> Tokens { get; set; }

        public ApplicationContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
        {
            public ApplicationContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                optionsBuilder.UseNpgsql(@$"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost"};
                        Port=5432;
                        Database={Environment.GetEnvironmentVariable("POSTGRES_NAME") ?? "backend_db2"};
                        Username={Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "backend_user"};
                        Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "backend_pass"}");

                return new ApplicationContext(optionsBuilder.Options);
            }
        }
    }
}
