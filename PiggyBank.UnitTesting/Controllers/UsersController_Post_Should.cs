using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using PiggyBank.Controllers;
using PiggyBank.Models;
using PiggyBank.UnitTesting.Mocks;

namespace PiggyBank.UnitTesting.Controllers
{
    public class UsersController_Post_Should
    {
        [Fact]
        public void ReturnUnknownWhenUserHasBeenCreatedSuccessfully()
        {
            MockPiggyBankRepository repository = new MockPiggyBankRepository(null);
            UsersController controller = new UsersController();
            controller.Repo = repository;

            // create user
            User user = new User();
            user.Email = "foo@bar.com";
            user.Name = "foo";
            IActionResult result = controller.Post(user);

            Assert.True(result.GetType() == typeof(CreatedAtRouteResult));
        }
    }

}