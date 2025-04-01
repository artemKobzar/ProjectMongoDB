using DuendeIdentity.Models;
using Duende.IdentityServer.Services;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Azure.Security.KeyVault.Secrets;
using System.Security;

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
            // Create Logger
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<Program>();
            X509Certificate2 signingCertificate = null; 
            try
            {
                logger.LogInformation("Starting certificate retrieval from Azure Key Vault...");
                // Load the certificate from Azure Key Vault
                var keyVaultUrl = builder.Configuration["AzureKeyVault:Url"];
                var certificateName = builder.Configuration["AzureKeyVault:CertificateName"]; // The name used in Key Vault
                if (string.IsNullOrEmpty(keyVaultUrl) || string.IsNullOrEmpty(certificateName))
                {
                    logger.LogError("Key Vault URL or Certificate Name is missing from configuration.");
                    throw new InvalidOperationException("Azure Key Vault configuration is missing.");
                }
                logger.LogInformation($"Connecting to Key Vault: {keyVaultUrl}");

                // Authenticate with Azure Key Vault
                var credential = new DefaultAzureCredential();
                var secretClient = new SecretClient(new Uri(keyVaultUrl), credential);

                logger.LogInformation($"Fetching certificate: {certificateName}");
                var secret = secretClient.GetSecret(certificateName);

                if (secret.Value == null || string.IsNullOrEmpty(secret.Value.Value))
                {
                    logger.LogError($"Certificate {certificateName} not found in Key Vault.");
                    throw new InvalidOperationException($"Certificate {certificateName} is missing or empty.");
                }
                logger.LogInformation("Certificate successfully retrieved from Key Vault.");

                // Convert Base64 to Byte Array
                var pfxBytes = Convert.FromBase64String(secret.Value.Value);
                logger.LogInformation("Certificate successfully converted from Base64.");
                signingCertificate = new X509Certificate2(pfxBytes, (string?)null, X509KeyStorageFlags.MachineKeySet);

                logger.LogInformation($"Certificate loaded successfully. Subject: {signingCertificate.Subject}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error retrieving the certificate: {ex.Message}");
                throw; // Re-throw the exception so the app fails fast if critical
            }

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

// Load certificate from file
//var signingCertPath = builder.Configuration["SigningCertificate:Path"];
//var signingCertPassword = builder.Configuration["SigningCertificate:Password"];
//var signingCertificate = new X509Certificate2(signingCertPath, signingCertPassword);