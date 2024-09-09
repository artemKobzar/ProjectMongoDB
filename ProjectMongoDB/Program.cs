using ProjectMongoDB.DbContext;
using ProjectMongoDB.Repositories;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityServer4.AccessTokenValidation;
using Microsoft.OpenApi.Models;
using Amazon.Runtime.Internal.Transform;
using ProjectMongoDB.Services;
using ProjectMongoDB;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Description = "Swagger API",
        Title = "Swagger IdentityServer4",
        Version = "1.0.0",
    });
});

builder.Services.Configure<DbSettings>( builder.Configuration.GetSection("MyDb"));
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IPassportUserRepository, PassportUserRepository>();
builder.Services.AddTransient<IUserImageRepository, UserImageRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7353";
        options.TokenValidationParameters.ValidateAudience = false;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "ProjectMongoDB");
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger UI");
        //options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        //options.OAuthClientId("project-test-api");
        //options.OAuthClientSecret("secret");
        //options.OAuthUsePkce();
    });
}

app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();

//builder.Services.AddAuthentication(options =>
//    {
//        options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
//        options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
//        options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
//    })
//    .AddIdentityServerAuthentication(options =>
//    {
//        options.ApiName = "ProjectMongoDB";
//        options.Authority = "https://localhost:7255";
//        options.RequireHttpsMetadata = false;
//    });
//app.UseHttpsRedirection();
//app.MapControllers();

//builder.Services.AddAuthentication("Bearer")
//    .AddIdentityServerAuthentication("Bearer", options =>
//    {
//        options.ApiName = "ProjectMongoDB";
//        options.Authority = "https://localhost:7353";
//        options.RequireHttpsMetadata = false;

//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateAudience = false,
//            NameClaimType = "name",
//            RoleClaimType = "role"
//        };
//    });

// Swagger options to configure Authorization Code
//options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
//{
//    Type = SecuritySchemeType.OAuth2,
//    Flows = new OpenApiOAuthFlows
//    {
//        AuthorizationCode = new OpenApiOAuthFlow
//        {
//            AuthorizationUrl = new Uri("https://localhost:7255/connect/authorize"),
//            TokenUrl = new Uri("https://localhost:7255/connect/token"),
//            Scopes = new Dictionary<string, string>
//                {
//                    {"ProjectMongoDB", "Web API" }
//                }
//        }
//    }
//});
//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "oath2"
//                },
//                Scheme = "oath2",
//                Name = "Bearer",
//                In = ParameterLocation.Header
//            },
//            new List<string>()
//        }
//    });
//    options.OperationFilter<AuthorizeCheckOperationFilter>();
//});