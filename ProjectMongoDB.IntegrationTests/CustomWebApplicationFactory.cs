using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectMongoDB.Repositories;
using MongoDB.Driver;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using ProjectMongoDB.DbContext;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace ProjectMongoDB.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup>: WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

                services.RemoveAll(typeof(IConfigureOptions<DbSettings>));
                // Replace with test database configuration
                services.Configure<DbSettings>(options =>
                {
                    options.ConnectionString = "mongodb://localhost:27017";
                    options.DbName = "TestDatabase";  // Use the test database
                    options.UsersCollectionName = "Users";
                    options.PassportUsersCollectionName = "PassportUsers";
                    options.UsersImageCollectionName = "UsersImage";
                });
                builder.Configure(app =>
                {
                    app.UseAuthentication();
                    app.UseAuthorization();
                });
                services.AddTransient<IUserRepository, UserRepository>();
                services.AddTransient<IPassportUserRepository, PassportUserRepository>();
            });
        }
    }
}
