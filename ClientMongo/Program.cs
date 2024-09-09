using ClientMongo.Services;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.IdentityModel.Tokens.Jwt;

namespace ClientMongo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ITokenService, TokenService>();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies", options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.LoginPath = "/Auth/Login";
                    options.AccessDeniedPath = "/Auth/AccessDenied";
                    options.SlidingExpiration = true;
                })
                .AddOpenIdConnect("oidc",
                options =>
                {
                    options.Authority = builder.Configuration["InteractiveServiceSettings:AuthorityUrl"];
                    options.ClientId = builder.Configuration["InteractiveServiceSettings:ClientId"];
                    options.ClientSecret = builder.Configuration["InteractiveServiceSettings:ClientSecret"];
                    options.Scope.Add(builder.Configuration["InteractiveServiceSettings:Scopes:0"]);
                    options.Scope.Add("roles");
                    //options.ClaimActions.MapJsonKey("role", "role");
                    options.ClaimActions.Add(new JsonKeyClaimAction("role", null, "role"));
                    options.TokenValidationParameters.RoleClaimType = "role";
                    options.TokenValidationParameters.NameClaimType = "name";
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.RequireHttpsMetadata = false;

                    options.ResponseType = "code";
                    options.UsePkce = true;
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRemoteFailure = context =>
                        {
                            context.Response.Redirect("/");
                            context.HandleResponse();
                            return Task.FromResult(0);
                        }
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapBlazorHub().RequireAuthorization();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}
