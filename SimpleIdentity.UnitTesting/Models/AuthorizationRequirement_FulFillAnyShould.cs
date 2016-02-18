using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using SimpleIdentity.Models;
using SimpleIdentity.UnitTesting.Mocks;

namespace SimpleIdentity.UnitTesting.Models
{
    public class AuthorizationRequirement_FulFillAnyShould
    {
        [Fact]
        public async void ReturnTrue_WhenUserFulfillRequirements()
        {
            MockSimpleIdentityDbContext dbContext = new MockSimpleIdentityDbContext(MockData.Seed());
            SimpleIdentityRepository repo = new SimpleIdentityRepository(dbContext);
            List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = 1,
                Scopes = Authorization.AuthScopes.Readable
            });
            bool result = await AuthorizationRequirement.FulfillAny(repo, "token1", reqs);

            Assert.True(result);
        }
    }
}
