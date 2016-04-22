using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SimpleIdentity.Models
{
    public class AuthorizationManager : IAuthorizationManager
    {
        private ISimpleIdentityDbContext _dbContext;
        private int _currentUserId;
        private List<AuthorizationRequirement> _fulfilledRequirements;

        public AuthorizationManager(ISimpleIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Authorization> GrantAuthorizationToUser(
            Authorization.AuthResourceType resourceType,
            int resourceId,
            Authorization.AuthScopes scopes,
            User user)
        {
            Authorization auth = new Authorization
            {
                ResourceType = resourceType,
                ResourceId = resourceId,
                Scopes = scopes,
                User = user,
                GrantDate = DateTime.Now
            };
            _dbContext.Authorizations.Add(auth);
            await _dbContext.SaveChangesAsync();

            return auth;
        }

        public async Task<bool> IsUserAuthorized(
            int userId,
            Authorization.AuthResourceType resourceType,
            int resourceId,
            Authorization.AuthScopes scopes)
        {
            if (_currentUserId != userId)
            {
                _currentUserId = userId;
                _fulfilledRequirements = new List<AuthorizationRequirement>();
            }
            if (_fulfilledRequirements.Any(req => req.AuthResourceType == resourceType &&
               req.ResourceId == resourceId &&
               req.Scopes.HasFlag(scopes)))
            {
                return true;
            }
            bool dbResult = await _dbContext.Authorizations
                .Where(b => b.User.Id == userId &&
                    b.ResourceType == resourceType &&
                    b.ResourceId == resourceId &&
                    b.Scopes.HasFlag(scopes) &&
                    !b.IsRevoked &&
                    b.User.IsActive)
                .AnyAsync();
            if (dbResult)
            {
                _fulfilledRequirements.Add(new AuthorizationRequirement
                {
                    AuthResourceType = resourceType,
                    ResourceId = resourceId,
                    Scopes = scopes
                });
            }
            return dbResult;
        }
    }
}
