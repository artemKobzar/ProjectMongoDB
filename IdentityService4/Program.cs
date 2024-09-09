using IdentityService4;
using IdentityService4.Data;
using IdentityService4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AutoMapper.Configuration;
using System.Reflection;

namespace IdentityService4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigurationManager configuration = builder.Configuration;
            // Add services to the container.
            builder.Services.ConfigureIdentityServices(builder.Configuration);

            builder.Services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(option =>
                 {
                    option.ConfigureDbContext = c => c.UseSqlServer(builder.Configuration.GetConnectionString("SQLDbConnectionIdentity4"),
                        sql => sql.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name));
                 })
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = o => o.UseSqlServer(builder.Configuration.GetConnectionString("SQLDbConnectionIdentity4"),
                        sql => sql.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name));
                })
                .AddDeveloperSigningCredential();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
//var seed = args.Contains("/seed");
//SeedData.EnsureSeedData(builder.Configuration.GetConnectionString("SQLDbConnectionIdentity4"));
//if (seed)
//{
//    args = args.Except(new[] { "/seed" }).ToArray();

//}