using DuendeIdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DuendeIdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.ConfigureIdentityServices(builder.Configuration);
            builder.Services.AddRazorPages();
            builder.Services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(option =>
                {
                    option.ConfigureDbContext = c => c.UseSqlServer(builder.Configuration.GetConnectionString("SQLDuendeConnectionIdentity"),
                        sql => sql.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name));
                })
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = o => o.UseSqlServer(builder.Configuration.GetConnectionString("SQLDuendeConnectionIdentity"),
                        sql => sql.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name));
                })
                .AddDeveloperSigningCredential();
            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.MapRazorPages();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}