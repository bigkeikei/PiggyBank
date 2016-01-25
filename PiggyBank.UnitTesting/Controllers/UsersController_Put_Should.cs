using Microsoft.AspNet.Mvc;
using Xunit;

using PiggyBank.Controllers;
using PiggyBank.Models;
using PiggyBank.UnitTesting.Mocks;

namespace PiggyBank.UnitTesting.Controllers
{
    public class UsersController_Put_Should
    {
        [Fact]
        public void ReturnHttpUnauthorized_WhenIdNotMatch()
        {
            MockPiggyBankRepository repository = new MockPiggyBankRepository(null);
            UsersController userController = new UsersController();
            userController.Repo = repository;

            User user1 = new User();
            user1.Name = "foo";
            user1.Email = "foo@bar.com";
            userController.Post(user1);

            User user2 = new User();
            user2.Name = "foo2";
            user2.Email = "foo2@bar.com";
            userController.Post(user2);

            IActionResult result = userController.Put(user2.Id, "Bearer abc", user1);

            Assert.True(result.GetType() == typeof(HttpUnauthorizedResult));
        }

        [Fact]
        public void ReturnNoContentResult_WhenSuccessfull()
        {
            MockPiggyBankRepository repository = new MockPiggyBankRepository(null);
            UsersController userController = new UsersController();
            TokensController tokenController = new TokensController();
            userController.Repo = repository;
            tokenController.Repo = repository;

            User user = new User();
            user.Name = "foo";
            user.Email = "foo@bar.com";
            userController.Post(user);
            tokenController.GetChallenge(user.Name);
            tokenController.GetToken(user.Name, user.Authentication.Signature);

            IActionResult result = userController.Put(user.Id, "Bearer " + user.Authentication.AccessToken, user);

            Assert.True(result.GetType() == typeof(NoContentResult));
        }
    }
}
