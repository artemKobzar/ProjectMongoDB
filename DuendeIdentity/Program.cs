using DuendeIdentity.Models;
using Duende.IdentityServer.Services;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;

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

            // Load the certificate from Azure Key Vault
            var keyVaultUrl = builder.Configuration["AzureKeyVault:Url"];
            var credential = new DefaultAzureCredential();
            var certificateClient = new CertificateClient(new Uri(keyVaultUrl), credential);

            var certificateName = "DuendeSigningCertificate"; // The name used in Key Vault
            var certificate = certificateClient.GetCertificate(certificateName);

            var signingCertificate = new X509Certificate2(certificate.Value.Cer);

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
                .AddSigningCredential(signingCertificate).AddProfileService<ProfileService>();
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