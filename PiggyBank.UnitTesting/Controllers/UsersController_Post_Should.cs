using Microsoft.AspNet.Mvc;
using Xunit;

using PiggyBank.Controllers;
using PiggyBank.Models;
using PiggyBank.UnitTesting.Mocks;

namespace PiggyBank.UnitTesting.Controllers
{
    public class UsersController_Post_Should
    {
        [Fact]
        public void ReturnCreatedAtRoute_WhenSuccessfull()
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
            Assert.True(((CreatedAtRouteResult)result).RouteName == "GetUser");
            Assert.True((string)((CreatedAtRouteResult)result).RouteValues["controller"] == "users");
            Assert.True((int)((CreatedAtRouteResult)result).RouteValues["userid"] == user.Id);
        }
    }

}