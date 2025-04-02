using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DuendeIdentity.Data
{
    public class IdentityDbContextContextFactory : IDesignTimeDbContextFactory<MongoDDuendeIdentityDbContext>
    {
        public MongoDDuendeIdentityDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.DuendeIdentity.json")
                .Build();
            var builder = new DbContextOptionsBuilder<MongoDDuendeIdentityDbContext>();
            var connectionString = configuration.GetConnectionString("SQLDuendeConnectionIdentity");

            //builder.UseSqlite(connectionString);
            builder.UseSqlServer(connectionString);

            return new MongoDDuendeIdentityDbContext(builder.Options);
        }
    }
}
