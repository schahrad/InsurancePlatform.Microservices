using IdentityService.Models;
using IdentityService.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            var connectionString = configuration.GetConnectionString("InsurancePlatformDatabase")
                                   ?? throw new InvalidOperationException("Connection string 'InsurancePlatformDatabase' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            // ASP.NET Core Identity (cookie auth)
            services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";  // where OpenIddict redirects unauthenticated users
            });


            // OpenIddict configuration – server only (NO validation here)
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<ApplicationDbContext>();
                })
                // Server: token issuing
                .AddServer(options =>
                {
                    //set issuer to the base URL of IdentityService
                    var issuer = configuration["OpenIddict:Issuer"] ?? "https://localhost:7121/";
                    options.SetIssuer(new Uri(issuer, UriKind.Absolute));

                    // Endpoints
                    options.SetAuthorizationEndpointUris("/connect/authorize");
                    options.SetTokenEndpointUris("/connect/token");
                    options.SetEndSessionEndpointUris("/connect/logout");

                    // Allow interactive code flow + refresh tokens + client_credentials (for Gateway.Api)
                    options
                        .AllowAuthorizationCodeFlow()
                        .RequireProofKeyForCodeExchange()
                        .AllowRefreshTokenFlow()
                        .AllowClientCredentialsFlow();

                    // Dev certs + disable access token encryption
                    options
                        .AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate()
                        .DisableAccessTokenEncryption();

                    // Register scopes 
                    options.RegisterScopes(
                                             Scopes.OpenId,
                                             Scopes.Profile,
                                             Scopes.Email,
                                             "api"
                                         );

                    // ASP.NET Core host integration
                    options.UseAspNetCore()
                           .EnableAuthorizationEndpointPassthrough();
                           //.EnableTokenEndpointPassthrough()
                           //.EnableEndSessionEndpointPassthrough();
                });

            return services;
        }
    }
}


