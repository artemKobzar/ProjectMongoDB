using DuendeIdentity.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DuendeIdentity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var seed = args.Contains("/seed");
            //args = args.Except(new[] { "/seed" }).ToArray();
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.DuendeIdentity.json", optional: false, reloadOnChange: true);
            //SeedData.EnsureSeedData(builder.Configuration.GetConnectionString("SQLDuendeConnectionIdentity"));
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.ConfigureIdentityServices(builder.Configuration);
            builder.Services.AddRazorPages();
            builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
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
                .AddDeveloperSigningCredential().AddProfileService<ProfileService>();
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}