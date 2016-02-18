using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using SimpleIdentity.UnitTesting.Mocks;
using SimpleIdentity.Models;

namespace SimpleIdentity.UnitTesting.Models
{
    public class UserManager_FindUserByTokenShould
    {
        [Fact]
        public async void ReturnUser_WhenTokenIsValid()
        {
            MockSimpleIdentityDbContext dbContext = new MockSimpleIdentityDbContext(MockData.Seed());
            UserManager userManager = new UserManager(dbContext);
            User user = await userManager.FindUserByToken("token1");
            Assert.True(user != null);
            Assert.True(user.Id == 1);
        }
    }
}
