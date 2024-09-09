using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DuendeIdentityServer.Data
{
    public class IdentityDbContextContextFactory : IDesignTimeDbContextFactory<MongoDDuendeIdentityDbContext>
    {
        public MongoDDuendeIdentityDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<MongoDDuendeIdentityDbContext>();
            var connectionString = configuration.GetConnectionString("SQLDbConnectionIdentity4");

            //builder.UseSqlite(connectionString);
            builder.UseSqlServer(connectionString);

            return new MongoDDuendeIdentityDbContext(builder.Options);
        }
    }
}
