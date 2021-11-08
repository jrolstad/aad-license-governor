using System.Security.Claims;

namespace AzureAdLicenseGovernor.Tests.Shared.Extensions
{
    public static class AuthenticatedUserExtensions
    {
        public static void WithAuthenticatedUser(this TestBuilderBase builder,
            string name,
            params string[] roles)
        {
            var nameClaim = new Claim(builder.Context.Identity.AuthenticatedUser.NameClaimType, name);
            builder.Context.Identity.AuthenticatedUser.AddClaim(nameClaim);

            foreach (var role in roles)
            {
                var roleClaim = new Claim(builder.Context.Identity.AuthenticatedUser.RoleClaimType, role);
                builder.Context.Identity.AuthenticatedUser.AddClaim(roleClaim);
            }
        }
    }
}
