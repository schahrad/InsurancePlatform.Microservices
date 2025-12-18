using IdentityService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace IdentityService.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        // OpenIddict entity sets
        public DbSet<OpenIddictEntityFrameworkCoreApplication> OpenIddictApplications { get; set; } = default!;
        public DbSet<OpenIddictEntityFrameworkCoreAuthorization> OpenIddictAuthorizations { get; set; } = default!;
        public DbSet<OpenIddictEntityFrameworkCoreScope> OpenIddictScopes { get; set; } = default!;
        public DbSet<OpenIddictEntityFrameworkCoreToken> OpenIddictTokens { get; set; } = default!;
    }
}
