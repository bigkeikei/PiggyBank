using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PiggyBank.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api")]
    public class UsersController : Controller
    {

        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet("[controller]")]
        public IEnumerable<User> List()
        {
            return Repo.ListUsers();
        }

        [HttpGet("[controller]/{userId}", Name = "GetUser")]
        public IActionResult Get(int userId, [FromHeader] string authorization)
        {
            User user = TokenRequirement.Fulfill(Repo, userId, authorization);
            if (user == null) return HttpUnauthorized();
            return new ObjectResult(user);
        }

        [HttpGet("me")]
        public IActionResult Get([FromHeader] string authorization)
        {
            string token = authorization.Substring(7);
            User user = Repo.FindUserByToken(token);
            if (user == null) return HttpUnauthorized();
            return new ObjectResult(user);
        }

        [HttpPost("[controller]")]
        public IActionResult Post([FromBody]User user)
        {
            if (user == null) return HttpBadRequest();
            User userCreated = Repo.CreateUser(user);
            return CreatedAtRoute("GetUser", new { controller = "users", name = user.Name }, userCreated);
        }

        [HttpPut("[controller]/{userId}")]
        public IActionResult Put(int userId, [FromHeader] string authorization, [FromBody]User user)
        {
            User userToUpdate = TokenRequirement.Fulfill(Repo, userId, authorization);
            if (userToUpdate == null) return HttpUnauthorized();
            if (userToUpdate.Name != user.Name)
            {
                return HttpBadRequest(new { Error = "Updating User.Name is not supported" });
            }
            Repo.UpdateUser(user);
            return new NoContentResult();
        }
    }
}
