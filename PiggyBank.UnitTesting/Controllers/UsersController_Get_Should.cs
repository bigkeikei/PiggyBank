using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Xunit;

using PiggyBank.Models;
using PiggyBank.Controllers;
using PiggyBank.UnitTesting.Mocks;

namespace PiggyBank.UnitTesting.Controllers
{
   public class UsersController_Get_Should
    {
		[Fact]
		public void ReturnHttpUnauthorized_WhenIdNotExists()
        {
            MockPiggyBankRepository repository = new MockPiggyBankRepository(null);
            UsersController controller = new UsersController();
            controller.Repo = repository;

            IActionResult result = controller.Get(1,"");
            Assert.True(result.GetType() == typeof(HttpUnauthorizedResult));
        }

        [Fact]
        public void ReturnObjectResult_WhenIdAndTokenProvided()
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

            IActionResult result = userController.Get(user.Id, "Bearer " + user.Authentication.AccessToken);

            Assert.True(result.GetType() == typeof(ObjectResult));
            Assert.True(((ObjectResult)result).Value.GetType() == typeof(User));
            User userUpdated = (User)((ObjectResult)result).Value;
            Assert.True(user.Id == userUpdated.Id);
            Assert.True(user.Name == userUpdated.Name);
            Assert.True(user.Email == userUpdated.Email);
        }

        [Fact]
        public void ReturnObjectResult_WhenTokenProvided()
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

            IActionResult result = userController.Get("Bearer " + user.Authentication.AccessToken);

            Assert.True(result.GetType() == typeof(ObjectResult));
            Assert.True(((ObjectResult)result).Value.GetType() == typeof(User));
            User userUpdated = (User)((ObjectResult)result).Value;
            Assert.True(user.Id == userUpdated.Id);
            Assert.True(user.Name == userUpdated.Name);
            Assert.True(user.Email == userUpdated.Email);
        }
    }
}
