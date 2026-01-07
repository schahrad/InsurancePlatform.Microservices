using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Ensure appsettings in wwwroot are loaded
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json", optional: true, reloadOnChange: false);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var authority = builder.Configuration["Identity:Authority"] ?? "https://localhost:7121/";

builder.Services.AddOidcAuthentication(options =>
{
    //IdentityService base URL (issuer)
    options.ProviderOptions.Authority = authority;

    // Must match the ClientId seeded in IdentitySeed
    options.ProviderOptions.ClientId = "webclient";

    //correspond to your OpenIddict config
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
    options.ProviderOptions.DefaultScopes.Add("api");

    // Configure redirect URIs - must be relative paths
    options.ProviderOptions.RedirectUri = "authentication/login-callback";
    options.ProviderOptions.PostLogoutRedirectUri = "/";
    
    // Handle authentication failures more gracefully
    options.UserOptions.NameClaim = "name";
    options.UserOptions.RoleClaim = "role";
    
    // Configure authentication paths
    options.AuthenticationPaths.LogInPath = "authentication/login";
    options.AuthenticationPaths.LogInCallbackPath = "authentication/login-callback";
    options.AuthenticationPaths.LogOutPath = "authentication/logout";
    options.AuthenticationPaths.LogOutCallbackPath = "authentication/logout-callback";
    options.AuthenticationPaths.LogOutSucceededPath = "/";
    options.AuthenticationPaths.RegisterPath = "authentication/register";
    
    // Response mode for authorization code flow
    options.ProviderOptions.ResponseMode = "query";
});

await builder.Build().RunAsync();
