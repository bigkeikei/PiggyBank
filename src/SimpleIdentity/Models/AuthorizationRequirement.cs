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

        public async Task<bool> Fulfill(ISimpleIdentityRepository repo, string token)
        {
            User user = await repo.UserManager.FindUserByToken(token);
            return await Fulfill(repo, user.Id);
        }

        public static async Task<bool> FulfillAny(ISimpleIdentityRepository repo, string token, IEnumerable<AuthorizationRequirement> requirements)
        {
            User user = await repo.UserManager.FindUserByToken(token);
            foreach (var req in requirements)
            {
                if (await req.Fulfill(repo, user.Id)) return true;
            }
            return false;
        }
    }
}
