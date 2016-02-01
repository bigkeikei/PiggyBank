using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using PiggyBank.Models;
using PiggyBank.UnitTesting.Mocks;

namespace PiggyBank.UnitTesting.Models
{
    public class UserManager_FindUser_Should
    {
        [Fact]
        public void ThrowDataNotFoundException_WhenUserCannotBeFound()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockPiggyBankDbContext();
                UserManager userManager = new UserManager(mockDbContext);
                userManager.FindUser(1);
            }
            catch (PiggyBankDataNotFoundException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public void ReturnUser_WhenUserIdMatch()
        {
            MockData data = new MockData();
            data.Users.Add(new User { Id = 1, Name = "AAA" });
            data.Users.Add(new User { Id = 2, Name = "Skiny Pig" });
            data.Users.Add(new User { Id = 3, Name = "CCC" });
            User user = data.Users[1];
            var mockDbContext = new MockPiggyBankDbContext(data);
            UserManager userManager = new UserManager(mockDbContext);
            User found = userManager.FindUser(user.Id);

            Assert.True(found.Id == user.Id);
            Assert.True(found.Name == user.Name);
        }
    }
}
