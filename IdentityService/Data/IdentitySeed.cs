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
        }
    }
}



