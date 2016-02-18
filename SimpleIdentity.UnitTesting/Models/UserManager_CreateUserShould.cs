using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using System.Data.Entity;
using Xunit;

using SimpleIdentity.Models;
using SimpleIdentity.UnitTesting.Mocks;

namespace SimpleIdentity.UnitTesting.Models
{
    public class UserManager_CreateUserShould
    {
        [Fact]
        public async void ThrowDataException_WhenUserNotProvided()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockSimpleIdentityDbContext();
                UserManager userManager = new UserManager(mockDbContext);
                await userManager.CreateUser(null);
            }
            catch (SimpleIdentityDataException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public async void ThrowDataException_WhenMandatoryElementIsMissing()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockSimpleIdentityDbContext();
                UserManager userManager = new UserManager(mockDbContext);

                await userManager.CreateUser(new User());
            }
            catch (SimpleIdentityDataException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public async void ReturnUser_WhenSucessful()
        {
            var mockDbContext = new MockSimpleIdentityDbContext();
            UserManager userManager = new UserManager(mockDbContext);
            AuthorizationManager authManager = new AuthorizationManager(mockDbContext);

            User user = new User();
            user.Name = "foo";
            user.Email = "foo@bar.com";
            user.IsActive = true;
            user.Id = 1;

            var createdUser = await userManager.CreateUser(user);

            Assert.True(createdUser.Name == user.Name);
            Assert.True(createdUser.Email == user.Email);
            Assert.True(createdUser.Authentication.Secret.Length > 0);
            Assert.True(await authManager.IsUserAuthorized(1, Authorization.AuthResourceType.User, 1, Authorization.AuthScopes.Full));

        }
    }
}
