using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdentity.Models
{
    public interface IAuthorizationManager
    {
        Task<Authorization> GrantAuthorizationToUser(
            Authorization.AuthResourceType resourceType,
            int resourceId,
            Authorization.AuthScopes scopes,
            User user);
        Task<bool> IsUserAuthorized(
            int userId,
            Authorization.AuthResourceType resourceType,
            int resourceId,
            Authorization.AuthScopes scopes);
    }
}
