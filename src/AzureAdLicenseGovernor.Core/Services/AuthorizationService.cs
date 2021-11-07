using AzureAdLicenseGovernor.Core.Extensions;
using AzureAdLicenseGovernor.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AzureAdLicenseGovernor.Core.Services
{
    public class AuthorizationService
    {
        private readonly IConfiguration _configuration;

        private Dictionary<string, List<string>> _authorizedRoles = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase)
        {
            {AuthorizedActions.ViewDirectories,new List<string>{ Roles.DirectoryContributor } },
            {AuthorizedActions.ManageDirectories,new List<string>{ Roles.DirectoryContributor } },
            {AuthorizedActions.ViewGroups,new List<string>{ Roles.GroupContributor } },
            {AuthorizedActions.ManageGroups,new List<string>{ Roles.GroupContributor } },
            {AuthorizedActions.ApplyLicensing,new List<string>{ Roles.LicensingContributor } },
        };


        public AuthorizationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsAuthorized(string action, IEnumerable<ClaimsIdentity> identities)
        {
            if (_configuration.IsDevelopment()) return true;

            if (string.IsNullOrWhiteSpace(action)) return false;

            var hasAction = _authorizedRoles.TryGetValue(action.Trim(), out List<string> roles);

            if (!hasAction) return false;
            if (!roles.Any()) return true;

            var isAuthorizedForAction = identities?
                .Any(r => HasRole(r, roles)) ?? false;

            return isAuthorizedForAction;
        }

        private static bool HasRole(ClaimsIdentity identity, List<string> roles)
        {
            var userRoles = identity.Claims
                .Where(c => c.Type == identity.RoleClaimType || string.Equals(c.Type, "roles"))
                .Select(c => c.Value);

            var userIsInRole = userRoles
                .Join(roles, role1 => role1, role2 => role2, (role1, role2) => role1)
                .Any();

            return userIsInRole;

        }
    }
}
