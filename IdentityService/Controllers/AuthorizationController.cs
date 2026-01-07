using IdentityService.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityService.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorizationController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest()
                ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // 1) Check if the user is signed in with Identity cookie
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (result?.Principal is null)
            {
                // No cookie -> go to /Account/Login and come back here afterwards
                var props = new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                };

                return Challenge(props, IdentityConstants.ApplicationScheme);
            }

            // 2) Load user
            var user = await _userManager.GetUserAsync(result.Principal)
                ?? throw new InvalidOperationException("The user details cannot be retrieved.");

            // 3) Build identity used for tokens
            var identity = new ClaimsIdentity(
                authenticationType: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                nameType: Claims.Name,
                roleType: Claims.Role);


            identity.AddClaim(new Claim(Claims.Subject, await _userManager.GetUserIdAsync(user)));
            identity.AddClaim(new Claim(Claims.Name, user.Email ?? user.UserName ?? string.Empty));
            identity.AddClaim(new Claim(Claims.Email, user.Email ?? string.Empty));


            // add nonce so the SPA can validate the ID token
            if (!string.IsNullOrEmpty(request.Nonce))
            {
                identity.AddClaim(new Claim(Claims.Nonce, request.Nonce));
            }

            if (!string.IsNullOrEmpty(user.FirstName))
                identity.AddClaim(new Claim("given_name", user.FirstName));
            if (!string.IsNullOrEmpty(user.LastName))
                identity.AddClaim(new Claim("family_name", user.LastName));

            var principal = new ClaimsPrincipal(identity);

            // 4) scopes & resources
            principal.SetScopes(request.GetScopes());
            principal.SetResources(await GetResourcesAsync(principal.GetScopes()));

            // 5) Map all claims to both access & identity tokens (for demo)
            principal.SetDestinations(static claim =>
            {
                return
                [
                    Destinations.AccessToken,
                    Destinations.IdentityToken
                ];
            });

            // 6) Let OpenIddict issue the tokens
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        private Task<IEnumerable<string>> GetResourcesAsync(IEnumerable<string> scopes)
        {
            var resources = new[] { "api" };
            return Task.FromResult<IEnumerable<string>>(resources);
        }
    }
}
