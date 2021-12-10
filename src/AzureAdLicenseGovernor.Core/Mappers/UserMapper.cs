
namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class UserMapper
    {
        public Models.User Map(Models.Directory directory, Microsoft.Graph.DirectoryObject toMap)
        {
            if (toMap == null) return null;

            if (toMap is Microsoft.Graph.User user)
            {
                return new Models.User
                {
                    TenantId = directory.TenantId,
                    ObjectId = user?.Id,
                    UserPrincipalName = user?.UserPrincipalName
                };
            }

            return new Models.User
            {
                TenantId = directory.TenantId,
                ObjectId = toMap?.Id,
                UserPrincipalName = null
            };
            
        }
    }
}
