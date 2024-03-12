using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityService4.Data
{
    public class IdentityDbContextContextFactory : IDesignTimeDbContextFactory<ProjectMongoDBIdentityDbContext>
    {
        public ProjectMongoDBIdentityDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<ProjectMongoDBIdentityDbContext>();
            var connectionString = configuration.GetConnectionString("SQLDbConnectionIdentity4");

            //builder.UseSqlite(connectionString);
            builder.UseSqlServer(connectionString);

            return new ProjectMongoDBIdentityDbContext(builder.Options);
        }
    }
}
