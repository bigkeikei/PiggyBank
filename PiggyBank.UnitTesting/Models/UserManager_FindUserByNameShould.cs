using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using SimpleIdentity.Models;
using SimpleIdentity.UnitTesting.Mocks;

namespace SimpleIdentity.UnitTesting.Models
{
    public class UserManager_FindUserByNameShould
    {
        [Fact]
        public async void ThrowException_WhenUserNotFound()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockSimpleIdentityDbContext();
                UserManager userManager = new UserManager(mockDbContext);
                await userManager.FindUserByName("Some Name");
            }
            catch (SimpleIdentityDataNotFoundException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public async void ReturnUser_WhenUserNameMatch()
        {
            var mockDbContext = new MockSimpleIdentityDbContext(MockData.Seed());
            UserManager userManager = new UserManager(mockDbContext);

            var rand = new Random(Guid.NewGuid().GetHashCode());
            User user = mockDbContext.Data.Users[rand.Next(0, mockDbContext.Data.Users.Count - 1)];
            User found = await userManager.FindUserByName(user.Name);

            Assert.True(found.Id == user.Id);
            Assert.True(found.Name == user.Name);
        }
    }
}
