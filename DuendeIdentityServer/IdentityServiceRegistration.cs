using DuendeIdentityServer.Data;
using DuendeIdentityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuendeIdentityServer
{
    public static class IdentityServiceRegistration
    {
        public static IServiceCollection ConfigureIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MongoDDuendeIdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SQLDbConnectionIdentity4"),
                b => b.MigrationsAssembly(typeof(MongoDDuendeIdentityDbContext).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<MongoDDuendeIdentityDbContext>().AddDefaultTokenProviders();
            //services.AddTransient<IAuthService, AuthService>();
            return services;
        }
    }
}
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ClockSkew = TimeSpan.Zero,
//            ValidIssuer = configuration["JWTSettings:Issuer"],
//            ValidAudience = configuration["JWTSettings:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
//        };
//    });
//services.AddIdentityServer().
//    AddConfigurationStore(option =>
//    {
//        option.ConfigureDbContext = c => c.UseSqlServer(configuration.GetConnectionString("SQLDbConnectionIdentity4"),
//            sql => sql.MigrationsAssembly(typeof(ProjectMongoDBIdentityDbContext).Assembly.FullName));
//    })
//    .AddOperationalStore(opt =>
//    {
//        opt.ConfigureDbContext = o => o.UseSqlServer(configuration.GetConnectionString("SQLDbConnectionIdentity4"),
//            sql => sql.MigrationsAssembly(typeof(ProjectMongoDBIdentityDbContext).Assembly.FullName));
//    });
