using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using SimpleIdentity.UnitTesting.Mocks;
using SimpleIdentity.Models;

namespace SimpleIdentity.UnitTesting.Models
{
    public class TokenManager_GenerateTokenShould
    {
        [Fact]
        public async void ReturnToken_WhenSuccessfull()
        {
            MockSimpleIdentityDbContext dbContext = new MockSimpleIdentityDbContext(MockData.Seed());
            TokenManager tokenManager = new TokenManager(dbContext);
            Token token = await tokenManager.GenerateToken(2, "secret", 1, "123");
            Assert.True(token != null);
            Assert.True(token.User.Id == 2);
            Assert.True(token.Client.Id == 1);
            Assert.True(token.TokenTimeout >= DateTime.Now);
        }

    }
}
