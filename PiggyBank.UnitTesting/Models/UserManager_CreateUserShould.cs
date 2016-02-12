using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using System.Data.Entity;
using Xunit;

using PiggyBank.Models;
using PiggyBank.UnitTesting.Mocks;

namespace PiggyBank.UnitTesting.Models
{
    public class UserManager_CreateUserShould
    {
        [Fact]
        public async void ThrowDataException_WhenUserNotProvided()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockPiggyBankDbContext();
                UserManager userManager = new UserManager(mockDbContext);
                await userManager.CreateUser(null);
            }
            catch (PiggyBankDataException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public async void ThrowDataException_WhenMandatoryElementIsMissing()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockPiggyBankDbContext();
                UserManager userManager = new UserManager(mockDbContext);

                await userManager.CreateUser(new User());
            }
            catch (PiggyBankDataException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public async void ReturnUser_WhenSucessful()
        {
            var mockDbContext = new MockPiggyBankDbContext();
            UserManager userManager = new UserManager(mockDbContext);

            User user = new User();
            user.Name = "foo";
            user.Email = "foo@bar.com";

            var createdUser = await userManager.CreateUser(user);

            Assert.True(createdUser.Name == user.Name);
            Assert.True(createdUser.Email == user.Email);
        }
    }
}
