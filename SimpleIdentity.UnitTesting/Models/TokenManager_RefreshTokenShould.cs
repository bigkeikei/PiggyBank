using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using SimpleIdentity.Models;
using SimpleIdentity.UnitTesting.Mocks;

namespace SimpleIdentity.UnitTesting.Models
{
    public class TokenManager_RefreshTokenShould
    {
        [Fact]
        public async void ReturnToken_WhenSuccessfull()
        {
            MockSimpleIdentityDbContext dbContext = new MockSimpleIdentityDbContext(MockData.Seed());
            TokenManager tokenManager = new TokenManager(dbContext);
            Token token = await tokenManager.RefreshToken("token1", "refresh1");
            Assert.True(token != null);
            Assert.True(token.User.Id == 1);
            Assert.True(token.Client.Id == 1);
            Assert.True(token.TokenTimeout >= DateTime.Now);
            Assert.True(dbContext.SaveCount == 1);
        }
    }
}
