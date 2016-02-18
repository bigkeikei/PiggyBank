using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdentity.Models
{
    public class AuthorizationRequirement
    {
        public Authorization.AuthResourceType AuthResourceType { get; set; }
        public int ResourceId { get; set; }
        public Authorization.AuthScopes Scopes { get; set; }

        public async Task<bool> Fulfill(ISimpleIdentityRepository repo, int userId)
        {
            return await repo.AuthorizationManager.IsUserAuthorized(userId, AuthResourceType, ResourceId, Scopes);
        }

        public static async Task<bool> FulfillAny(ISimpleIdentityRepository repo, int userId, IEnumerable<AuthorizationRequirement> requirements)
        {
            foreach (var req in requirements)
            {
                if (await req.Fulfill(repo, userId)) return true;
            }
            return false;
        }
    }
}
