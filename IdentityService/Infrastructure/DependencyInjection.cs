using IdentityService.Models;
using IdentityService.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
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

            // ASP.NET Core Identity
            services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // OpenIddict configuration
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<ApplicationDbContext>();
                })
                // Server: token issuing
                .AddServer(options =>
                {
                    // Endpoints
                    options.SetTokenEndpointUris("/connect/token");

                    options.AllowClientCredentialsFlow();

                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();

                    // Register scopes if needed
                    options.RegisterScopes(Scopes.OpenId, "api");

                    // ASP.NET Core host integration
                    options.UseAspNetCore()
                           .EnableTokenEndpointPassthrough();
                })
                // Validation: so APIs can validate the tokens
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            // 4) Authentication/Authorization for this service
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });

            services.AddAuthorization();

            return services;
        }
    }
}


