using System.Collections.Generic;
using Microsoft.AspNet.Mvc;

using PiggyBank.Controllers.Exceptions;
using PiggyBank.Models;
using PiggyBank.Models.Data;

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
            return Repo.UserManager.ListUsers();
        }

        [HttpGet("[controller]/{userId}", Name = "GetUser")]
        public IActionResult Get(int userId, [FromHeader] string authorization)
        {
            try
            {
                return new ObjectResult(GetUser(userId, authorization));
            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("me")]
        public IActionResult Get([FromHeader] string authorization)
        {
            try
            {
                string token = authorization.Substring(7);
                User user = Repo.UserManager.FindUserByToken(token);
                if (user == null) return HttpUnauthorized();
                return new ObjectResult(user);
            }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPost("[controller]")]
        public IActionResult Post([FromBody]User user)
        {
            try
            {
                if (user == null) return HttpBadRequest();
                User userCreated = Repo.UserManager.CreateUser(user);
                return CreatedAtRoute("GetUser", new { controller = "users", name = user.Name }, userCreated);
            }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("[controller]/{userId}")]
        public IActionResult Put(int userId, [FromHeader] string authorization, [FromBody]User user)
        {
            try
            {
                User userToUpdate = GetUser(userId, authorization);
                Repo.UserManager.UpdateUser(user);
                return new NoContentResult();
            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        private User GetUser(int userId, string authorization)
        {
            User user = TokenRequirement.Fulfill(Repo, userId, authorization);
            if (user == null) throw new PiggyBankUserException("Unknown error");
            return user;
        }
    }
}
