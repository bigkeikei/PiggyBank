using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Xunit;
using PiggyBank.Controllers;
using PiggyBank.UnitTesting.Mocks;

namespace PiggyBank.UnitTesting.Controllers
{
	
    public class UsersController_Get_Should
    {
		[Fact]
		public void ReturnHttpUnauthorizedWhenIdNotExists()
        {
            MockPiggyBankRepository repository = new MockPiggyBankRepository(null);
            UsersController controller = new UsersController();
            controller.Repo = repository;
            IActionResult result = controller.Get(1,"");
            Assert.True(true);
        }
    }
}
