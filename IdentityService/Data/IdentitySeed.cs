using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;


namespace IdentityService.Data
{
    /// <summary>
    /// startup seeding helper for OpenIddict, not for normal users
    /// </summary>
    public static class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync("demo@local.test");
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = "demo@local.test",
                    Email = "demo@local.test",
                    EmailConfirmed = true,
                    FirstName = "Demo",
                    LastName = "User"
                };

                await userManager.CreateAsync(user, "Pass123$"); // for dev only
            }

            var scopeManager = services.GetRequiredService<IOpenIddictScopeManager>();
            var applicationManager = services.GetRequiredService<IOpenIddictApplicationManager>();

            // Create an API scope if it does not exist
            if (await scopeManager.FindByNameAsync("api") is null)
            {
                await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = "api",
                    DisplayName = "Main API scope"
                });
            }

            // Create a machine-to-machine client (for Gateway.Api)
            if (await applicationManager.FindByClientIdAsync("gateway_api") is null)
            {
                await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "gateway_api",
                    ClientSecret = "super-secret-gateway-password", // store securely later
                    DisplayName = "Gateway API client",
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.Prefixes.Scope + "api"
                    }
                });
            }

            // Blazor WebClient (public SPA client)
            if (await applicationManager.FindByClientIdAsync("webclient") is null)
            {
                await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "webclient",
                    DisplayName = "Blazor WebClient",
                    ClientType = ClientTypes.Public, // SPA cannot keep a secret

                    RedirectUris =
                        {
                            new Uri("https://localhost:7281/authentication/login-callback")
                        },
                    PostLogoutRedirectUris =
                        {
                            new Uri("https://localhost:7281/") // back to root after logout
                        },

                    Permissions =
        {
            Permissions.Endpoints.Authorization,
            Permissions.Endpoints.Token,
            Permissions.Endpoints.EndSession,
            Permissions.GrantTypes.AuthorizationCode,
            Permissions.GrantTypes.RefreshToken,
            Permissions.ResponseTypes.Code,
            //Permissions.Scopes.OpenId,
            Permissions.Prefixes.Scope + "openid",
            Permissions.Scopes.Profile,
            Permissions.Scopes.Email,
            Permissions.Prefixes.Scope + "api"
        },

                    Requirements =
        {
            Requirements.Features.ProofKeyForCodeExchange // PKCE
        }
                });
            }

        }
    }
}



